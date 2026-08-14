class FDCPAccordion {
    constructor(element) {
        this.element = element;
        this.accordionId = element.id;
        this.detailsInGroup = Array.from(element.querySelectorAll('gcds-details'));
        this.expandBtn = element.querySelector('[button-id="fdcp-accordion-expand-all-button"]');
        this.collapseBtn = element.querySelector('[button-id="fdcp-accordion-collapse-all-button"]');

        this.bindEvents();
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

            const isNotAlwaysOpen = this.element.classList.contains('fdcp-accordion-not-always-open');

            requestAnimationFrame(() => {
                if (isNotAlwaysOpen && clicked.hasAttribute('open')) {
                    this.detailsInGroup.forEach(other => {
                        if (other !== clicked && other.hasAttribute('open')) {
                            other.removeAttribute('open');
                        }
                    });
                }
            });
        });

        if (this.expandBtn) {
            this.expandBtn.addEventListener('click', () => this.openAll());
        }

        if (this.collapseBtn) {
            this.collapseBtn.addEventListener('click', () => this.closeAll());
        }
    };
    
    openAll() {
        this.element.dataset.bulkAction = 'true';
        this.detailsInGroup.forEach(details => details.setAttribute('open', ''));
        delete this.element.dataset.bulkAction;
    }

    closeAll() {
        this.element.dataset.bulkAction = 'true';
        this.detailsInGroup.forEach(details => details.removeAttribute('open'));
        delete this.element.dataset.bulkAction;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.fdcp-accordion').forEach(element => {
        if (!element.FDCPAccordionInstance) {
            element.FDCPAccordionInstance = new FDCPAccordion(element);
        }
    });
});

window.FDCPAccordion = FDCPAccordion;