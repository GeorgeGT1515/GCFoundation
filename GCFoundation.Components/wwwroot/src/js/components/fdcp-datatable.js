document.querySelectorAll('.tabulator-table').forEach(el => {
    const config = {
        layout: el.dataset.layout || "fitColumns",
        pagination: true,
        paginationSize: parseInt(el.dataset.paginationSize || "10"),
        columns: [],
        height: '100%'
    };

    // Parse columns
    if (el.dataset.columns) {
        try {
            config.columns = JSON.parse(el.dataset.columns);
        } catch (e) {
            console.error("Failed to parse columns:", e);
        }
    }

    if (el.dataset.set) {
        config.data = JSON.parse(el.dataset.set);
    } else if (el.dataset.ajaxurl) {
        config.ajaxURL = el.dataset.ajaxurl;
        config.filterMode = "remote";
        config.sortMode = "remote";
        config.ajaxConfig = "POST";
        config.ajaxContentType = "json";
        config.paginationMode = "remote";
        config.filterMode = "remote";

        // Add anti-forgery token to AJAX requests if available
        if (el.dataset.antiforgeryToken) {
            // Parse the anti-forgery token HTML to extract the token value
            const parser = new DOMParser();
            const tokenDoc = parser.parseFromString(el.dataset.antiforgeryToken, 'text/html');
            const tokenInput = tokenDoc.querySelector('input[name="__RequestVerificationToken"]');
            
            if (tokenInput) {
                const tokenValue = tokenInput.getAttribute('value');
                config.ajaxRequestFunc = function(url, config, params) {
                    return fetch(url, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': tokenValue
                        },
                        body: JSON.stringify(params)
                    }).then(response => response.json());
                };
            }
        }
    }

    // If a pagination element is provided, render pagination controls there
    let paginatorEl = null;
    if (el.dataset.paginationElement) {
        paginatorEl = document.getElementById(el.dataset.paginationElement);
        if (paginatorEl) {
            // Still provide the element to Tabulator to suppress in-table controls
            config.paginationElement = paginatorEl;
        }
    }

    const table = new Tabulator(el, config);

    // Hide/neutralize Tabulator's internal paginator in footer
    function suppressInternalPaginator() {
        const footer = el.querySelector('.tabulator-footer');
        if (footer) {
            const internalPag = footer.querySelector('.tabulator-paginator');
            if (internalPag) internalPag.replaceChildren();
            footer.setAttribute('aria-hidden', 'true');
            footer.style.display = 'none';
        }
    }

    // Build GC Design System gcds-pagination controls with fallback to anchors
    function renderCustomPagination() {
        if (!paginatorEl) return;

        paginatorEl.innerHTML = '';

        const totalPages = Math.max(1, table.getPageMax?.() || 1);
        const currentPage = Math.max(1, table.getPage?.() || 1);

        const gcdsDefined = typeof window !== 'undefined' && window.customElements && window.customElements.get && window.customElements.get('gcds-pagination');

        if (gcdsDefined) {
            const gcds = document.createElement('gcds-pagination');
            gcds.setAttribute('display', 'list');
            gcds.setAttribute('total-pages', String(totalPages));
            gcds.setAttribute('current-page', String(currentPage));
            gcds.setAttribute('previous-label', 'Previous page');
            gcds.setAttribute('next-label', 'Next page');
            const url = { queryStrings: { 'page::match': '{{#}}' }, fragment: '' };
            try { gcds.setAttribute('url', JSON.stringify(url)); } catch {}
            paginatorEl.appendChild(gcds);
        } else {
            // Fallback: accessible anchors following GC DS semantics
            const nav = document.createElement('nav');
            nav.setAttribute('aria-label', 'Table pagination');
            const list = document.createElement('ul');
            list.className = 'fdcp-pagination-list';

            const createItem = (label, page, options = {}) => {
                const li = document.createElement('li');
                const a = document.createElement('a');
                a.href = '#';
                a.textContent = label;
                a.setAttribute('data-page', String(page));
                if (options.rel) a.setAttribute('rel', options.rel);
                if (options.ariaLabel) a.setAttribute('aria-label', options.ariaLabel);
                if (options.disabled) { a.setAttribute('aria-disabled', 'true'); a.tabIndex = -1; }
                if (options.current) a.setAttribute('aria-current', 'page');
                li.appendChild(a);
                return li;
            };

            const firstDisabled = currentPage === 1;
            list.appendChild(createItem('First', 'first', { rel: 'first', ariaLabel: 'First page', disabled: firstDisabled }));
            list.appendChild(createItem('Prev', 'prev', { rel: 'prev', ariaLabel: 'Previous page', disabled: firstDisabled }));

            const windowSize = 5;
            let start = Math.max(1, currentPage - 2);
            let end = Math.min(totalPages, start + windowSize - 1);
            start = Math.max(1, end - windowSize + 1);
            for (let p = start; p <= end; p++) {
                list.appendChild(createItem(String(p), p, { current: p === currentPage, ariaLabel: `Show page ${p}` }));
            }

            const lastDisabled = currentPage === totalPages;
            list.appendChild(createItem('Next', 'next', { rel: 'next', ariaLabel: 'Next page', disabled: lastDisabled }));
            list.appendChild(createItem('Last', 'last', { rel: 'last', ariaLabel: 'Last page', disabled: lastDisabled }));

            nav.appendChild(list);
            paginatorEl.appendChild(nav);
        }

        suppressInternalPaginator();
    }

    // Wire gcds-pagination interactions to Tabulator
    if (paginatorEl) {
        paginatorEl.addEventListener('click', (e) => {
            const path = e.composedPath();
            const anchor = path.find?.(n => n && n.tagName === 'A');
            if (!anchor) return;
            const a = anchor;
            const ariaDisabled = a.getAttribute?.('aria-disabled');
            if (ariaDisabled === 'true') { e.preventDefault(); return; }
            const ariaLabel = a.getAttribute?.('aria-label') || '';
            // Try to infer page from aria-label or href
            let target = null;
            const m = ariaLabel.match(/(page|Page)\s+(first|prev|next|last|\d+)/);
            if (m) target = m[2];
            if (!target && a.href) {
                const url = new URL(a.href, window.location.origin);
                const qp = url.searchParams.get('page');
                if (qp) target = qp;
            }
            if (!target) return;
            e.preventDefault();
            switch (target) {
                case 'first':
                    table.setPage(1);
                    break;
                case 'prev':
                    table.previousPage?.();
                    break;
                case 'next':
                    table.nextPage?.();
                    break;
                case 'last':
                    table.setPage(table.getPageMax?.() || 1);
                    break;
                default:
                    table.setPage(parseInt(target, 10));
            }
        });
        // Listen for potential custom change events from gcds component
        paginatorEl.addEventListener('gcdsChange', (e) => {
            const detail = e.detail || {};
            const page = detail.currentPage || detail.page;
            if (page) table.setPage(parseInt(page, 10));
        });
    }

    // Accessibility: ensure container has correct role/structure expectations
    // Make the grid container focusable and remove focusability from the inner holder
    el.setAttribute('role', 'grid');
    el.setAttribute('tabindex', '0');

    const applyA11yFixes = () => {
        const holder = el.querySelector('.tabulator-tableholder');
        if (holder && holder.hasAttribute('tabindex')) {
            holder.removeAttribute('tabindex');
        }

        // Remove nested rowgroup role on header contents to avoid rowgroup-in-rowgroup
        const headerContents = el.querySelector('.tabulator-header-contents');
        if (headerContents && headerContents.getAttribute('role') === 'rowgroup') {
            headerContents.removeAttribute('role');
        }

        // Hide internal Tabulator alert and footer from a11y tree; surface status to external live region
        const internalAlert = el.querySelector('.tabulator-alert-msg');
        if (internalAlert) {
            internalAlert.setAttribute('aria-hidden', 'true');
            internalAlert.removeAttribute('role');
            const statusId = el.dataset.statusElement;
            if (statusId) {
                const statusEl = document.getElementById(statusId);
                if (statusEl) {
                    statusEl.textContent = internalAlert.textContent || '';
                }
            }
        }
    };

    // Apply immediately and observe for dynamic changes from Tabulator
    applyA11yFixes();
    const mo = new MutationObserver(() => applyA11yFixes());
    mo.observe(el, { childList: true, subtree: true, attributes: true, attributeFilter: ['tabindex', 'role'] });

    // Re-render custom pagination when Tabulator page changes or data loads
    const rerender = () => renderCustomPagination();
    table.on?.('pageLoaded', rerender);
    table.on?.('dataProcessed', rerender);
    table.on?.('dataLoaded', rerender);
    table.on?.('dataChanged', rerender);
    renderCustomPagination();
});

document.querySelectorAll('.tabulator-search-input').forEach(el => {
    el.addEventListener("input", debounce(function (e) {
        let tabulatorId = e.target.dataset.tabulatorId;
        let table = Tabulator.findTable("#" + tabulatorId)[0];
        const value = e.target.value.trim();

        if (!table) return;

        if (value === "") {
            table.clearFilter();
        } else {
            // Get the table element to access filterable fields
            const tableElement = document.getElementById(tabulatorId);
            const filterableFields = JSON.parse(tableElement.dataset.filterableFields || '[]');

            table.setFilter(
                filterableFields.map(field => ({ field: field, type: "like", value: value }))
            );
        }
    }, 200)); // 200ms debounce
});

function debounce(func, wait) {
    let timeout;
    return function (...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}
