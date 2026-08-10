class FDCPAccordion {
    constructor(element) {
        this.element = element;
        this.accordionId = element.id;
        this.detailsInGroup = Array.from(element.querySelectorAll('gcds-details'));
        this.toggleBtn = element.querySelector('.fdcp-accordion-toggle');

        const lang = document.documentElement.lang?.startsWith('fr') ? 'fr' : 'en';
        this.strings = ACCORDION_TOGGLE_STRINGS[lang];

        this.bindEvents();

        // Always start on "Open all" regardless of initial panel state
        if (this.toggleBtn) {
            this.toggleBtn.textContent = this.strings.openAll;
        }
    }

    bindEvents() {
        this.element.addEventListener('click', event => {
            if (this.element.dataset.bulkAction === 'true') {
                return;
            }

            const clicked = event.target.closest('gcds-details');
            if (!clicked || !this.detailsInGroup.includes(clicked)) {
                return;
            }

            // gcds-details toggles its own 'open' attribute internally on click;
            // wait a frame so we read its state after that toggle has happened.
            requestAnimationFrame(() => {
                if (clicked.hasAttribute('open')) {
                    this.detailsInGroup.forEach(other => {
                        if (other !== clicked && other.hasAttribute('open')) {
                            other.removeAttribute('open');
                        }
                    });
                }

                // Individual clicks can only ever move toward "all closed" —
                // opening one always closes the rest, so "all open" is only
                // reachable via the toggle button itself. Only check for the
                // all-closed case here; no need to flip to "Close all".
                const allClosed = this.detailsInGroup.every(details => !details.hasAttribute('open'));
                if (allClosed && this.toggleBtn) {
                    this.toggleBtn.textContent = this.strings.openAll;
                }
            });
        });

        if (this.toggleBtn) {
            this.toggleBtn.addEventListener('click', () => {
                const isOpenAllMode = this.toggleBtn.textContent === this.strings.openAll;

                if (isOpenAllMode) {
                    this.openAll();
                } else {
                    this.closeAll();
                }
            });
        }
    }

    openAll() {
        this.element.dataset.bulkAction = 'true';
        this.detailsInGroup.forEach(details => details.setAttribute('open', ''));
        delete this.element.dataset.bulkAction;

        if (this.toggleBtn) {
            this.toggleBtn.textContent = this.strings.closeAll;
        }
    }

    closeAll() {
        this.element.dataset.bulkAction = 'true';
        this.detailsInGroup.forEach(details => details.removeAttribute('open'));
        delete this.element.dataset.bulkAction;

        if (this.toggleBtn) {
            this.toggleBtn.textContent = this.strings.openAll;
        }
    }
}

const ACCORDION_TOGGLE_STRINGS = {
    en: { openAll: 'Open all', closeAll: 'Close all' },
    fr: { openAll: 'Tout ouvrir', closeAll: 'Tout fermer' }
};

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.fdcp-accordion').forEach(element => {
        if (!element.FDCPAccordionInstance) {
            element.FDCPAccordionInstance = new FDCPAccordion(element);
        }
    });
});

window.FDCPAccordion = FDCPAccordion;