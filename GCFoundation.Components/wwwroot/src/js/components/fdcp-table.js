class FDCPTable {
    constructor(element) {
        this.table = element;
        this.observer = new MutationObserver((mutations) => {
            const hasRowSpans = mutations.some(m =>
                Array.from(m.addedNodes).some(n =>
                    n.nodeType === 1 && n.matches?.('span[slot]')
                )
            );
            if (hasRowSpans) {
                this.dispatchRowsRendered();
            }
        });
        this.observer.observe(this.table, { childList: true, subtree: true });
    }

    dispatchRowsRendered() {
        document.dispatchEvent(new CustomEvent('fdcp-table:rows-rendered', {
            detail: { table: this.table }
        }));
    }

    destroy() {
        this.observer?.disconnect();
    }
}

document.querySelectorAll('gcds-table').forEach(el => new FDCPTable(el));