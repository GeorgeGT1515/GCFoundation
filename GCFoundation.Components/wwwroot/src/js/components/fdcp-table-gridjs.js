(() => {
    'use strict';

    const global = window;

    function parseJsonSafe(str, fallback = null) {
        if (!str) return fallback;
        // Decode HTML entities that may be present in data attributes
        const decoded = str.replace(/&quot;/g, '"').replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
        try { return JSON.parse(decoded); } catch { return fallback; }
    }

    function addOrReplaceParams(url, params) {
        const u = new URL(url, window.location.origin);
        for (const [k, v] of Object.entries(params)) {
            if (v === undefined || v === null || v === '') {
                u.searchParams.delete(k);
            } else {
                u.searchParams.set(k, v);
            }
        }
        return u.toString();
    }

    function createLiveRegion(container) {
        const live = document.createElement('div');
        live.setAttribute('role', 'status');
        live.setAttribute('aria-live', 'polite');
        live.setAttribute('aria-atomic', 'true');
        // Make visually hidden but accessible if no CSS utility present
        live.style.position = 'absolute';
        live.style.width = '1px';
        live.style.height = '1px';
        live.style.margin = '-1px';
        live.style.border = '0';
        live.style.padding = '0';
        live.style.clip = 'rect(0 0 0 0)';
        live.style.overflow = 'hidden';
        container.prepend(live);
        return live;
    }

    function updateAriaSortForState(container, sortState) {
        const headers = container.querySelectorAll('thead th.gridjs-th');
        
        headers.forEach((h, index) => {
            // Ensure scope="col" is always set for accessibility
            if (!h.hasAttribute('scope')) h.setAttribute('scope', 'col');

            // Check if this header is sortable (has the gridjs-th-sort class)
            if (h.classList.contains('gridjs-th-sort')) {
                // This header is sortable
                // Check if this column is currently sorted
                if (sortState && sortState.columnIndex === index) {
                    // This column is sorted
                    if (sortState.direction === 1) {
                        h.setAttribute('aria-sort', 'ascending');
                    } else if (sortState.direction === -1) {
                        h.setAttribute('aria-sort', 'descending');
                    } else {
                        h.setAttribute('aria-sort', 'none');
                    }
                } else {
                    // Sortable but not currently sorted
                    h.setAttribute('aria-sort', 'none');
                }
            } else {
                // Non-sortable headers should not have aria-sort
                h.removeAttribute('aria-sort');
            }
        });

        updateSortButtonLabels(container);
    }

    function getSortButtonLabel(header) {
        const columnLabel = header.querySelector('.gridjs-th-content')?.textContent?.trim()
            || header.textContent?.trim()
            || 'Column';
        const ariaSort = header.getAttribute('aria-sort');
        let sortState = 'sortable';
        if (ariaSort === 'ascending') {
            sortState = 'sorted ascending';
        } else if (ariaSort === 'descending') {
            sortState = 'sorted descending';
        } else if (ariaSort === 'none') {
            sortState = 'not sorted';
        }
        return `${columnLabel}, ${sortState}`;
    }

    function updateSortButtonLabels(container) {
        container.querySelectorAll('th.gridjs-th-sort').forEach((header) => {
            const sortButton = header.querySelector('button');
            if (!sortButton) {
                return;
            }
            sortButton.setAttribute('aria-label', getSortButtonLabel(header));
        });
    }

    function enhanceSortableHeaders(root, getSortState) {
        // Non-sortable headers must not be in the tab order.
        root.querySelectorAll('thead th[tabindex]').forEach((header) => {
            if (!header.classList.contains('gridjs-th-sort')) {
                header.removeAttribute('tabindex');
                header.removeAttribute('role');
            }
        });

        root.querySelectorAll('th.gridjs-th-sort').forEach((header) => {
            // Grid.js renders a <button> inside the header; keep that as the only tab stop.
            header.removeAttribute('tabindex');
            header.removeAttribute('role');

            const sortButton = header.querySelector('button');
            if (!sortButton) {
                return;
            }

            if (!sortButton.hasAttribute('tabindex')) {
                sortButton.setAttribute('tabindex', '0');
            }

            sortButton.setAttribute('aria-label', getSortButtonLabel(header));

            if (sortButton.dataset.fdcpSortEnhanced === 'true') {
                return;
            }
            sortButton.dataset.fdcpSortEnhanced = 'true';

            const refreshSortState = () => updateAriaSortForState(root, getSortState());

            sortButton.addEventListener('click', () => {
                window.setTimeout(refreshSortState, 100);
                window.setTimeout(refreshSortState, 300);
                window.setTimeout(refreshSortState, 600);
                window.setTimeout(refreshSortState, 1000);
            });

            sortButton.addEventListener('keydown', (e) => {
                if (e.key !== ' ' && e.key !== 'Enter') {
                    return;
                }
                e.preventDefault();
                sortButton.click();
                window.setTimeout(refreshSortState, 100);
                window.setTimeout(refreshSortState, 300);
                window.setTimeout(refreshSortState, 600);
                window.setTimeout(refreshSortState, 1000);
            });
        });
    }

    function applyTableAccessibleName(root, tableEl, cfg) {
        if (!tableEl) return;

        const captionId = root.id ? `${root.id}-caption` : null;
        const captionEl = captionId ? document.getElementById(captionId) : null;
        const summaryEl = root.id ? document.getElementById(`${root.id}-summary`) : null;

        // <caption> is not a valid owned element of role="grid" (Grid.js sets role="grid" on the table).
        const injectedCaption = tableEl.querySelector('caption');
        if (injectedCaption) {
            injectedCaption.remove();
        }

        const wrapper = root.querySelector('.gridjs-wrapper');

        if (captionEl && captionId) {
            tableEl.setAttribute('aria-labelledby', captionId);
            if (wrapper) {
                wrapper.setAttribute('aria-labelledby', captionId);
                wrapper.removeAttribute('aria-label');
            }
            // Prefer labelledby over a duplicate aria-label on the grid table.
            tableEl.removeAttribute('aria-label');
        } else {
            if (cfg.ariaLabel && !tableEl.hasAttribute('aria-label')) {
                tableEl.setAttribute('aria-label', String(cfg.ariaLabel));
            }
            if (wrapper && cfg.ariaLabel && !wrapper.hasAttribute('aria-label')) {
                wrapper.setAttribute('aria-label', String(cfg.ariaLabel));
            }
        }

        if (summaryEl && summaryEl.id) {
            tableEl.setAttribute('aria-describedby', summaryEl.id);
            if (wrapper) {
                wrapper.setAttribute('aria-describedby', summaryEl.id);
            }
        }
    }

    function updateRowHeaders(container, columnDefs) {
        // Convert configured (isRowHeader) data cells into row header cells (<th scope="row">) in tbody
        // NOTE: Ensure that no duplicate <th>s are <td>s are left in the table.
        const rows = container.querySelectorAll('tbody tr');
        const rowHeaders = columnDefs.filter(col => col.isRowHeader);
        if (!rowHeaders.length) return;

        rows.forEach(row => {
            const orderedCells = Array.from(row.querySelectorAll('td, th'));
            orderedCells.forEach((cell, cellIndex) => {
                const column = columnDefs[cellIndex];
                if (!column || !column.id) return;
                if (!cell.hasAttribute('data-column-id')) {
                    cell.setAttribute('data-column-id', column.id);
                }
            });

            rowHeaders.forEach(rowHeader => {
                // Check if the row header has already been converted to <th scope="row">; also, ensure there are no duplicate <td>s.
                const existingHeaderCell = row.querySelector('th[data-column-id="' + rowHeader.id + '"]');
                const cells = Array.from(row.querySelectorAll('td[data-column-id="' + rowHeader.id + '"]'));
                if (existingHeaderCell) {
                    if (!existingHeaderCell.hasAttribute('scope')) existingHeaderCell.setAttribute('scope', 'row');
                    if (existingHeaderCell.classList.contains('gridjs-td')) {
                        existingHeaderCell.className = existingHeaderCell.className.replace(/\bgridjs-td\b/g, 'gridjs-th');
                    }
                    // Remove any stray TDs for this column-id (prevents an empty TD after TH)
                    cells.forEach(td => td.remove());
                    return;
                }
                if (cells.length === 0) return;

                // Create a new header cell (i.e. <th>).
                const cell = cells[0];
                const headerCell = document.createElement('th');
                Array.from(cell.attributes).forEach(attr => {
                    if (attr.name === 'class') {
                        headerCell.setAttribute('class', attr.value.replace(/\bgridjs-td\b/g, 'gridjs-th'));
                    } else {
                        headerCell.setAttribute(attr.name, attr.value);
                    }
                });
                while (cell.firstChild) { headerCell.appendChild(cell.firstChild); }
                headerCell.setAttribute('scope', 'row');

                // Replace the <td> with the new <th>. Remove any additional <td>s.
                cell.replaceWith(headerCell);
                cells.slice(1).forEach(td => td.remove());
            });
        });
    }

    function buildGrid(root) {
        if (!global.gridjs) {
            console.error('Grid.js is not available');
            return;
        }; // safety: initAll will retry when gridjs becomes available

        const cfg = parseJsonSafe(root.getAttribute('data-fdcp-grid')) || {};
        const dataUrl = cfg.dataUrl || cfg['data-url'] || root.getAttribute('data-url');
        if (!dataUrl) return;

        const columnsInput = Array.isArray(cfg.columns) ? cfg.columns : [];
        const localization = cfg.localization;
        const pageSize = Number(cfg.pageSize || 25);
        const paginationEnabled = cfg.paginationEnabled !== false;
        const searchEnabled = cfg.searchEnabled !== false;
        const sortingEnabled = cfg.sortingEnabled !== false;

        const columnDefs = columnsInput.map(col => ({
            id: col.id || col.name,
            name: col.name || col.id || '',
            hidden: col.isHidden || false,
            isRowHeader: col.isRowHeader || false,
            width: col.width || ''
            // Note: 'sort' property is for client-side sorting only
            // For server-side, all columns are sortable if server.sort is configured
            // Individual column sortability will be controlled via the sortingEnabled flag and col.isSortable check below
        }));
        const columnIdByIndex = columnDefs.map(c => c.id);

        // Track sort state for aria-sort attributes (since server-side sorting doesn't add CSS classes)
        let currentSortState = { columnIndex: null, direction: null }; // direction: 1 for asc, -1 for desc
        const getSortState = () => currentSortState;

        const liveRegion = createLiveRegion(root);

        const grid = new global.gridjs.Grid({
            columns: columnDefs,
            server: {
                url: dataUrl,
                then: data => {
                    const items = (data && Array.isArray(data.items)) ? data.items : [];
                    const page = Number((data && data.page != null) ? data.page : 1);
                    const pageSizeResp = Number((data && data.pageSize != null) ? data.pageSize : pageSize);
                    const total = Number((data && data.total != null) ? data.total : items.length);
                    liveRegion.textContent = `${total} results, page ${page}, ${pageSizeResp} per page.`;
                    return items.map(item => columnIdByIndex.map(function (cid) {
                        if (item && Object.prototype.hasOwnProperty.call(item, cid)) {
                            return item[cid];
                        }
                        return undefined;
                    }))
                },
                total: data => Number((data && data.total != null) ? data.total : 0)
            },
            search: searchEnabled ? {
                enabled: true,
                server: {
                    url: (prev, keyword) => addOrReplaceParams(prev, { q: (keyword == null ? '' : keyword) })
                }
            } : false,
            sort: sortingEnabled ? {
                multiColumn: false,
                server: {
                    url: (prev, columns) => {
                        if (!columns || !columns.length) {
                            // No sort applied - reset state
                            currentSortState = { columnIndex: null, direction: null };
                            return prev;
                        }
                        const col = columns[0];
                        const index = col.index;
                        const dir = col.direction; // 1 for asc, -1 for desc
                        const colId = columnIdByIndex[index];
                        
                        // Store sort state for aria-sort updates
                        currentSortState = { columnIndex: index, direction: dir };
                        
                        return addOrReplaceParams(prev, { sortBy: colId, sortDir: dir === 1 ? 'asc' : 'desc' });
                    }
                }
            } : false,
            pagination: paginationEnabled ? {
                limit: pageSize,
                server: {
                    url: (prev, page, limit) => addOrReplaceParams(prev, { page, pageSize: limit })
                }
            } : false,
            language: {
                search: { placeholder: cfg.localization.searchPlaceholder || 'Search...' },
                sort: {
                    sortAsc: cfg.localization.sortAscending || 'Sort column ascending',
                    sortDesc: cfg.localization.sortDescending || 'Sort column descending',
                },
                pagination: { 
                    previous: cfg.localization.paginationPrevious || 'Previous', 
                    next: cfg.localization.paginationNext || 'Next', 
                    showing: cfg.localization.paginationShowing || 'Showing',
                    of: cfg.localization.paginationOf || 'of',
                    to: cfg.localization.paginationTo || 'to',
                    results: () => cfg.localization.paginationResults || 'results' 
                },
                loading: cfg.localization.loadingText || 'Loading...',
                noRecordsFound: cfg.localization.noResultsText || cfg.localization.noDataText || 'No records found',
                error: cfg.localization.errorFetchText || 'An error happened while fetching the data'
            }
        });

        // Create a dedicated mount point inside root
        let mount = root.querySelector('.fdcp-gridjs-mount');
        if (!mount) {
            mount = document.createElement('div');
            mount.className = 'fdcp-gridjs-mount';
            root.appendChild(mount);
        }
        grid.render(mount);

        // Listen to Grid.js events for updates
        grid.on('ready', () => {
            const tableEl = root.querySelector('table.gridjs-table');
            applyTableAccessibleName(root, tableEl, cfg);
            updateAriaSortForState(root, currentSortState);
            updateRowHeaders(root, columnDefs);
            enhanceSortableHeaders(root, getSortState);
        });

        grid.on('sort', () => {
            // Multiple checks to ensure we catch the update
            setTimeout(() => updateAriaSortForState(root, currentSortState), 100);
            setTimeout(() => updateAriaSortForState(root, currentSortState), 300);
            setTimeout(() => updateAriaSortForState(root, currentSortState), 600);
        });

        // Post-render hooks
        setTimeout(() => {
            // Apply GC Design System table classes and ARIA
            const tableEl = root.querySelector('table.gridjs-table');
            if (tableEl) {
                tableEl.classList.add('fdcp-table');
                if (cfg.class && typeof cfg.class === 'string') {
                    cfg.class.split(/\s+/).forEach(c => c && tableEl.classList.add(c));
                }
                // Ensure an id for aria-controls linkage
                if (!tableEl.id) tableEl.id = `${root.id || 'fdcp-grid'}-table`;

                applyTableAccessibleName(root, tableEl, cfg);

                // Remove Grid.js's default aria-live summary (we have our own)
                const gridSummary = root.querySelector('.gridjs-summary');
                if (gridSummary) {
                    gridSummary.removeAttribute('role');
                    gridSummary.removeAttribute('aria-live');
                    gridSummary.setAttribute('aria-hidden', 'true');
                }

                // Add scope="col" to all column headers for accessibility
                const headers = tableEl.querySelectorAll('thead th');
                headers.forEach(th => {
                    if (!th.hasAttribute('scope')) {
                        th.setAttribute('scope', 'col');
                    }
                });
            }

            // Debounce search input
            if (searchEnabled) {
                const input = root.querySelector('.gridjs-search input');
                if (input) {
                    // Label the input for SR users and link to table
                    const label = cfg.localization.searchAriaLabel || 'Search table';
                    input.setAttribute('aria-label', label);
                    const tableFor = root.querySelector('table.gridjs-table');
                    if (tableFor && tableFor.id) input.setAttribute('aria-controls', tableFor.id);

                    // Grid.js already handles search via the built-in search box
                    // No need to debounce manually - Grid.js handles this
                }
            }
            
            updateAriaSortForState(root, currentSortState);
            enhanceSortableHeaders(root, getSortState);
        }, 0);

        // Keep aria-sort and row headers in sync when table changes (on re-render/sort/pagination)
        const observerConfig = {
            subtree: true,
            attributes: true,
            attributeFilter: ['class'],
            childList: true
        };

        let refreshScheduled = false;
        const mo = new MutationObserver((mutations) => {
            // Only react to relevant table mutations
            const hasTableChanges = mutations.some(m => {
                const t = m.target;
                return (t && (
                    t.tagName === 'TBODY' ||
                    t.tagName === 'THEAD' ||
                    (t.classList && (
                        t.classList.contains('gridjs-table') ||
                        t.classList.contains('gridjs-tbody') ||
                        t.classList.contains('gridjs-th')
                    ))
                ));
            });

            if (!hasTableChanges) return;

            if (refreshScheduled) return;
            refreshScheduled = true;

            // Defer to allow Grid.js to finish DOM updates, and avoid re-entrancy by disconnecting
            setTimeout(() => {
                refreshScheduled = false;
                mo.disconnect();
                try {
                    const tableEl = root.querySelector('table.gridjs-table');
                    applyTableAccessibleName(root, tableEl, cfg);
                    updateAriaSortForState(root, currentSortState);
                    updateRowHeaders(root, columnDefs);
                    enhanceSortableHeaders(root, getSortState);
                } finally {
                    mo.observe(root, observerConfig);
                }
            }, 0);
        });
        mo.observe(root, observerConfig);
    }

    function initAll() {
        const nodes = document.querySelectorAll('.fdcp-gridjs-container[data-fdcp-grid]');
        
        // Exit early if no grid tables found on page
        if (nodes.length === 0) {
            return;
        }
        
        if (!global.gridjs) {
            // Retry shortly until Grid.js is available
            setTimeout(initAll, 50);
            return;
        }
        
        nodes.forEach(node => buildGrid(node));
    }

    // Initialize on DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initAll);
    } else {
        initAll();
    }

    global.TableGridJs = { initAll, init: buildGrid };
})();