// Custom scripts for GCFoundationWeb
const GCFoundationWeb = {
    Version: "0.1.1",

    init: function () {
        // Example: Add copy functionality to code blocks
        const codeBlocks = document.querySelectorAll('pre code');
        codeBlocks.forEach(block => {
            block.addEventListener('click', function () {
                const text = this.textContent;
                navigator.clipboard.writeText(text).then(() => {
                    // Show a brief "copied" message
                    const originalText = this.textContent;
                    this.textContent = 'Copied!';
                    setTimeout(() => {
                        this.textContent = originalText;
                    }, 1000);
                });
            });
        });

        this.initTableActions();
    },
    initTableActions: function () {
        document.addEventListener("click", (e) => {
            const deleteBtn = e.target.closest('gcds-button[data-action="delete"]');
            if (!deleteBtn) return;

            e.preventDefault();

            const rowId = deleteBtn.row?.submissionId;
            console.log("Delete clicked for row:", rowId, deleteBtn.row);
        });
    }
};

// Initialize custom functionality when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('Custom scripts loaded for GCFoundationWeb');
    
    // Add any custom initialization here
    GCFoundationWeb.init();
});