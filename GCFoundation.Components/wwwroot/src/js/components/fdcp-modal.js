class FDCPModal {
    constructor(element) {
        this.modal = element;
        this.isStatic = element.dataset.static === 'true';
        this.closeButtons = element.querySelectorAll('.fdcp-modal-close');
        this.backdrop = element.querySelector('.modal-overlay__backdrop');
        this.triggerElement = null;
        this.inertedSiblings = [];
        this.wasOpen = element.classList.contains('show');

        this.bindEvents();
        this.observeVisibility();
    }

    bindEvents() {
        this.closeButtons.forEach(btn => {
            btn.addEventListener('click', () => this.hide());
        });

        if (!this.isStatic) {
            this.backdrop.addEventListener('click', () => this.hide());
        }

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && !this.isStatic && this.modal.classList.contains('show')) {
                this.hide();
            }
        });
    }

    // Watches the modal's class attribute so the enter/exit behavior runs
    // even if a developer toggles `.show` on the element directly, without
    // going through this.show()/this.hide().
    observeVisibility() {
        const observer = new MutationObserver(() => {
            const isOpenNow = this.modal.classList.contains('show');
            if (isOpenNow && !this.wasOpen) {
                this.onEnter();
            } else if (!isOpenNow && this.wasOpen) {
                this.onExit();
            }
            this.wasOpen = isOpenNow;
        });

        observer.observe(this.modal, { attributes: true, attributeFilter: ['class'] });
    }

    onEnter() {
        this.triggerElement = this.triggerElement || document.activeElement;
        this.modal.setAttribute('aria-hidden', 'false');
        document.body.style.overflow = 'hidden';

        this.inertedSiblings = [];
        let current = this.modal;

        while (current.parentElement) {
            const parent = current.parentElement;
            Array.from(parent.children).forEach((sibling) => {
                if (sibling !== current && !sibling.hasAttribute('inert')) {
                    sibling.setAttribute('inert', '');
                    this.inertedSiblings.push(sibling);
                }
            });
            current = parent;
            if (current === document.body) break;
        }

        const focusTarget =
            this.modal.querySelector('.fdcp-modal-close') ||
            this.modal.querySelector('.modal__footer gcds-button, .modal__footer button') ||
            this.modal;

        requestAnimationFrame(() => {
            if (typeof focusTarget.focus === 'function') {
                focusTarget.focus();
            }
        });
    }

    onExit() {
        this.modal.setAttribute('aria-hidden', 'true');
        document.body.style.overflow = '';

        this.inertedSiblings.forEach((el) => el.removeAttribute('inert'));
        this.inertedSiblings = [];

        if (this.triggerElement && typeof this.triggerElement.focus === 'function') {
            this.triggerElement.focus();
        }
        this.triggerElement = null;
    }

    show(triggerElement) {
        this.triggerElement = triggerElement || document.activeElement;
        this.modal.classList.add('show');
        // onEnter() fires automatically via the MutationObserver
    }

    hide() {
        this.modal.classList.remove('show');
        // onExit() fires automatically via the MutationObserver
    }
}

const fdcpModalRegistry = new Map();

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.modal-overlay').forEach(modalEl => {
        fdcpModalRegistry.set(modalEl.getAttribute('modal-id'), new FDCPModal(modalEl));
    });

    document.querySelectorAll('.fdcp-modal-open[modal-id]').forEach(trigger => {
        trigger.addEventListener('click', () => {
            const targetId = trigger.getAttribute('modal-id');
            const instance = fdcpModalRegistry.get(targetId);
            if (instance) {
                instance.show(trigger);
            }
        });
    });
});