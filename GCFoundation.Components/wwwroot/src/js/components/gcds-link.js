// Additional functionality for <gcds-link>.
const GCFoundation = GCFoundation || {};
GCFoundation.GCDSLink = GCFoundation.GCDSLink || {
    Version: "0.1.0",

    init: function () {
        // Example: Add smooth scrolling to anchor links.
        const anchorLinks = document.querySelectorAll('a[href^="#"]');
        anchorLinks.forEach(link => {
            link.addEventListener('click', function (e) {
                if (GCFoundation.GCDSLink.scrollToTarget(this.getAttribute('href'))) {
                    e.preventDefault();
                }
            });
        });

        window.addEventListener('load', GCFoundation.GCDSLink.scrollToHashTarget);
        window.addEventListener('hashchange', GCFoundation.GCDSLink.scrollToHashTarget);

        // Dispatch a custom event to signal that the component is ready.
        document.dispatchEvent(new CustomEvent("GCFoundation.GCDSLink-ready"));
    },

    getHashTarget: function (hash) {
        if (!hash)
            return null;

        const targetId = decodeURIComponent(hash.replace(/^#/, ''));
        return document.getElementById(targetId);
    },

    scrollToHashTarget: function () {
        // Wait briefly for GCDS components and images to finish layout before scrolling.
        GCFoundation.GCDSLink.scrollToTarget(window.location.hash, 150);
    },
    scrollToTarget: function (hash, delay = 0) {
        const targetElement = GCFoundation.GCDSLink.getHashTarget(hash);

        if (!targetElement) {
            return false;
        }

        setTimeout(() => {
            targetElement.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }, delay);

        return true;
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function () { GCFoundation.GCDSLink.init(); });