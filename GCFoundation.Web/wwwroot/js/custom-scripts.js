// Custom scripts for GCFoundationWeb
const GCFoundationWeb = {
    Version: "0.1.1",

    init: function () {
        this.initCopyCodeBlock();
        this.initTableActions();
    },
    initTableActions: function () {
        document.addEventListener("click", (e) => {
            const deleteBtn = e.target.closest('.delete');
            if (!deleteBtn) return;

            e.preventDefault();

            console.log("Delete clicked for row:", deleteBtn.row, "submisisonId:", deleteBtn.row?.submissionId);
        });
    },
    initCopyCodeBlock: function () {
        document.addEventListener("click", (e) => {
            const copyButton = e.target.closest('.code-copy-button');
            if (!copyButton) return;

            const container = copyButton.closest('pre') || copyButton.parentElement;
            const codeEl = container.querySelector('code');
            if (!codeEl) return;

            const text = codeEl.textContent;
            navigator.clipboard.writeText(text).then(() => {
                const originalText = copyButton.textContent;
                const successText = copyButton.dataset.successText || 'Copied!';
                copyButton.textContent = successText;
                setTimeout(() => {
                    copyButton.textContent = originalText;
                }, 1000);
            });
        });
    }
};

// Initialize custom functionality when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('Custom scripts loaded for GCFoundationWeb');
    
    // Add any custom initialization here
    GCFoundationWeb.init();
});