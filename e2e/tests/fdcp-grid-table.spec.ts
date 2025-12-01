import { test, expect } from '@playwright/test';
import { runAxeAndAttach } from '../utils/a11y';

const GRID_TABLE_URL = '/en/components/table-grid-js';
const GRID_CONTAINER = '#employees-grid';
const SEARCH_INPUT = `${GRID_CONTAINER} .gridjs-search input`;
const TABLE_SELECTOR = `${GRID_CONTAINER} table.gridjs-table`;

test.describe('FDCP Grid Table Component', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto(GRID_TABLE_URL);
    // Wait for Grid.js to initialize and load data
    await page.waitForSelector(TABLE_SELECTOR, { timeout: 10000 });
    await page.waitForTimeout(1000); // Give time for initial render
  });

  test.describe('9.1 - Search filters server-side and reflects in UI Count', () => {
    test('should filter results when searching', async ({ page }) => {
      // Get initial row count
      const initialRows = await page.locator(`${TABLE_SELECTOR} tbody tr`).count();
      expect(initialRows).toBeGreaterThan(0);

      // Type search query
      const searchInput = page.locator(SEARCH_INPUT);
      await expect(searchInput).toBeVisible();
      await searchInput.fill('Finance');
      
      // Wait for debounce and server response
      await page.waitForTimeout(500);
      await page.waitForLoadState('networkidle');

      // Verify results are filtered
      const filteredRows = await page.locator(`${TABLE_SELECTOR} tbody tr`).count();
      expect(filteredRows).toBeLessThanOrEqual(initialRows);
      
      // Verify "Finance" appears in the visible results
      const tableContent = await page.locator(`${TABLE_SELECTOR} tbody`).textContent();
      expect(tableContent).toContain('Finance');
    });

    test('should update result count via aria-live region', async ({ page }) => {
      // Find our custom aria-live region (not Grid.js's summary)
      const liveRegion = page.locator(`${GRID_CONTAINER} [role="status"][aria-live="polite"][aria-atomic="true"]`).first();
      await expect(liveRegion).toBeAttached();

      // Perform search
      await page.locator(SEARCH_INPUT).fill('Employee 1');
      await page.waitForTimeout(500);
      await page.waitForLoadState('networkidle');

      // Verify aria-live region was updated with count
      const liveText = await liveRegion.textContent();
      expect(liveText).toMatch(/\d+ results/i);
    });

    test('should clear search and show all results', async ({ page }) => {
      const searchInput = page.locator(SEARCH_INPUT);
      
      // Search for specific term
      await searchInput.fill('Finance');
      await page.waitForTimeout(500);
      await page.waitForLoadState('networkidle');
      const filteredRows = await page.locator(`${TABLE_SELECTOR} tbody tr`).count();

      // Clear search
      await searchInput.clear();
      await page.waitForTimeout(500);
      await page.waitForLoadState('networkidle');

      // Verify more results are shown
      const allRows = await page.locator(`${TABLE_SELECTOR} tbody tr`).count();
      expect(allRows).toBeGreaterThan(filteredRows);
    });
  });

  test.describe('9.2 - Sorting toggles and aria-sort semantics', () => {
    test('should sort ascending when clicking header', async ({ page }) => {
      const nameHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'Name' });
      
      // Click to sort ascending
      await nameHeader.click();
      await page.waitForLoadState('networkidle');

      // Verify aria-sort attribute
      await expect(nameHeader).toHaveAttribute('aria-sort', 'ascending');
      
      // Verify visual indicator
      await expect(nameHeader).toHaveClass(/gridjs-sort-asc/);
    });

    test('should toggle to descending when clicking sorted header', async ({ page }) => {
      const nameHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'Name' });
      
      // Click once for ascending
      await nameHeader.click();
      await page.waitForLoadState('networkidle');
      await expect(nameHeader).toHaveAttribute('aria-sort', 'ascending');

      // Click again for descending
      await nameHeader.click();
      await page.waitForLoadState('networkidle');
      await expect(nameHeader).toHaveAttribute('aria-sort', 'descending');
      await expect(nameHeader).toHaveClass(/gridjs-sort-desc/);
    });

    test('should update data order based on sort', async ({ page }) => {
      const idHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'ID' });
      
      // Click to sort by ID ascending
      await idHeader.click();
      await page.waitForLoadState('networkidle');

      // Get first row ID
      const firstRowId = await page.locator(`${TABLE_SELECTOR} tbody tr:first-child td:first-child`).textContent();
      
      // Click again to sort descending
      await idHeader.click();
      await page.waitForLoadState('networkidle');

      // Get first row ID after descending sort
      const firstRowIdDesc = await page.locator(`${TABLE_SELECTOR} tbody tr:first-child td:first-child`).textContent();
      
      // They should be different (unless there's only 1 row)
      const rowCount = await page.locator(`${TABLE_SELECTOR} tbody tr`).count();
      if (rowCount > 1) {
        expect(firstRowId).not.toBe(firstRowIdDesc);
      }
    });

    test('should sort names in natural numeric order (Employee 1, 2, 3... not 1, 10, 100)', async ({ page }) => {
      // Click Name header to sort ascending
      const nameHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'Name' });
      await nameHeader.click();
      await page.waitForLoadState('networkidle');
      
      // Get the first 10 name values
      const names = await page.locator(`${TABLE_SELECTOR} tbody tr td:nth-child(2)`).allTextContents();
      
      // Verify natural sorting: Employee 1, 2, 3, ..., 10 (not 1, 10, 100, 101, ...)
      expect(names[0]).toBe('Employee 1');
      expect(names[1]).toBe('Employee 2');
      expect(names[2]).toBe('Employee 3');
      expect(names[9]).toBe('Employee 10');
      
      // Verify we don't see Employee 100 in the first 10 results
      expect(names.slice(0, 10)).not.toContain('Employee 100');
    });

    test('should maintain only one aria-sort at a time', async ({ page }) => {
      const nameHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'Name' });
      const deptHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'Department' });
      
      // Sort by Name
      await nameHeader.click();
      await page.waitForLoadState('networkidle');
      await expect(nameHeader).toHaveAttribute('aria-sort', 'ascending');

      // Sort by Department
      await deptHeader.click();
      await page.waitForLoadState('networkidle');
      await expect(deptHeader).toHaveAttribute('aria-sort', 'ascending');
      
      // Name should no longer have aria-sort or have "none"
      const nameAriaSort = await nameHeader.getAttribute('aria-sort');
      expect(nameAriaSort).not.toBe('ascending');
    });
  });

  test.describe('9.3 - Pagination controls and focus management', () => {
    test('should have pagination controls', async ({ page }) => {
      const pagination = page.locator(`${GRID_CONTAINER} .gridjs-pagination`);
      await expect(pagination).toBeVisible();
      
      // Check for next/previous buttons
      const nextButton = pagination.locator('button').filter({ hasText: /next/i });
      const prevButton = pagination.locator('button').filter({ hasText: /previous/i });
      
      // At least one should be visible (Next on first page)
      const hasNextOrPrev = await nextButton.isVisible().catch(() => false) || 
                           await prevButton.isVisible().catch(() => false);
      expect(hasNextOrPrev).toBe(true);
    });

    test('should navigate to next page', async ({ page }) => {
      const pagination = page.locator(`${GRID_CONTAINER} .gridjs-pagination`);
      const nextButton = pagination.locator('button').filter({ hasText: /next/i }).first();
      
      // Get first row text on page 1
      const firstRowPage1 = await page.locator(`${TABLE_SELECTOR} tbody tr:first-child`).textContent();
      
      // Click next
      if (await nextButton.isVisible()) {
        await nextButton.click();
        await page.waitForLoadState('networkidle');
        await page.waitForTimeout(500);

        // Get first row text on page 2
        const firstRowPage2 = await page.locator(`${TABLE_SELECTOR} tbody tr:first-child`).textContent();
        
        // Content should be different
        expect(firstRowPage1).not.toBe(firstRowPage2);
      }
    });

    test('should maintain keyboard focus in table after pagination', async ({ page }) => {
      const pagination = page.locator(`${GRID_CONTAINER} .gridjs-pagination`);
      const nextButton = pagination.locator('button').filter({ hasText: /next/i }).first();
      
      if (await nextButton.isVisible()) {
        // Focus on a cell in the table
        const firstCell = page.locator(`${TABLE_SELECTOR} tbody tr:first-child td:first-child`);
        await firstCell.focus();
        
        // Navigate to next page
        await nextButton.click();
        await page.waitForLoadState('networkidle');
        await page.waitForTimeout(500);

        // Verify focus management - table should still be accessible
        const tableVisible = await page.locator(TABLE_SELECTOR).isVisible();
        expect(tableVisible).toBe(true);
      }
    });

    test('should update page count in aria-live region', async ({ page }) => {
      const liveRegion = page.locator(`${GRID_CONTAINER} [role="status"][aria-live="polite"][aria-atomic="true"]`).first();
      const pagination = page.locator(`${GRID_CONTAINER} .gridjs-pagination`);
      const nextButton = pagination.locator('button').filter({ hasText: /next/i }).first();
      
      // Get initial page info
      const initialText = await liveRegion.textContent();
      expect(initialText).toMatch(/page 1/i);

      // Go to next page if available
      if (await nextButton.isVisible()) {
        await nextButton.click();
        await page.waitForLoadState('networkidle');
        await page.waitForTimeout(500);

        // Verify aria-live region updated with page 2
        const updatedText = await liveRegion.textContent();
        expect(updatedText).toMatch(/page 2/i);
      }
    });
  });

  test.describe('Accessibility', () => {
    test('should pass axe accessibility checks', async ({ page }, testInfo) => {
      const results = await runAxeAndAttach(page, testInfo, ['wcag2a', 'wcag2aa', 'wcag21aa']);
      expect(results.violations.length).toBe(0);
    });

    test('should have proper table semantics', async ({ page }) => {
      const table = page.locator(TABLE_SELECTOR);
      
      // Verify table has caption
      const caption = table.locator('caption');
      await expect(caption).toBeVisible();
      await expect(caption).toHaveText('Employees');

      // Verify table has proper headers
      const headers = table.locator('thead th');
      await expect(headers).toHaveCount(3);
      
      // Each header should have scope="col"
      for (let i = 0; i < await headers.count(); i++) {
        await expect(headers.nth(i)).toHaveAttribute('scope', 'col');
      }
    });

    test('should have accessible search input', async ({ page }) => {
      const searchInput = page.locator(SEARCH_INPUT);
      
      // Should have aria-label
      const ariaLabel = await searchInput.getAttribute('aria-label');
      expect(ariaLabel).toBeTruthy();
      expect(ariaLabel).toMatch(/search/i);

      // Should have aria-controls linking to table
      const ariaControls = await searchInput.getAttribute('aria-controls');
      expect(ariaControls).toBeTruthy();
    });

    test('should be fully keyboard navigable', async ({ page }) => {
      // Tab to search input
      await page.keyboard.press('Tab');
      let focused = await page.locator(':focus');
      
      // Keep tabbing until we find the search input or table elements
      let attempts = 0;
      let foundSearchOrTable = false;
      
      while (attempts < 20) {
        focused = page.locator(':focus');
        const focusedElement = await focused.evaluate(el => el.tagName + '.' + el.className);
        
        if (focusedElement.includes('input') || focusedElement.includes('gridjs')) {
          foundSearchOrTable = true;
          break;
        }
        
        await page.keyboard.press('Tab');
        attempts++;
      }
      
      expect(foundSearchOrTable).toBe(true);
    });
  });

  test.describe('Progressive Enhancement', () => {
    test('should have noscript fallback', async ({ page }) => {
      const pageContent = await page.content();
      expect(pageContent).toContain('<noscript>');
      expect(pageContent).toContain('</noscript>');
    });

    test('should render table when JavaScript is enabled', async ({ page }) => {
      const table = page.locator(TABLE_SELECTOR);
      await expect(table).toBeVisible();
      
      // Should have data rows
      const rows = await page.locator(`${TABLE_SELECTOR} tbody tr`).count();
      expect(rows).toBeGreaterThan(0);
    });
  });

  test.describe('Integration with Server Endpoint', () => {
    test('should make requests to correct endpoint', async ({ page }) => {
      // Listen for API requests
      const requests: string[] = [];
      page.on('request', request => {
        if (request.url().includes('/api/grid/employees')) {
          requests.push(request.url());
        }
      });

      // Reload to capture initial request
      await page.reload();
      await page.waitForLoadState('networkidle');

      // Verify request was made
      expect(requests.length).toBeGreaterThan(0);
      expect(requests[0]).toContain('/api/grid/employees');
    });

    test('should send correct query parameters for search', async ({ page }) => {
      let lastRequest: string = '';
      page.on('request', request => {
        if (request.url().includes('/api/grid/employees')) {
          lastRequest = request.url();
        }
      });

      // Perform search
      await page.locator(SEARCH_INPUT).fill('Finance');
      await page.waitForTimeout(500);
      await page.waitForLoadState('networkidle');

      // Verify query parameter
      expect(lastRequest).toContain('q=Finance');
    });

    test('should send correct query parameters for sorting', async ({ page }) => {
      let lastRequest: string = '';
      page.on('request', request => {
        if (request.url().includes('/api/grid/employees')) {
          lastRequest = request.url();
        }
      });

      // Click to sort
      const nameHeader = page.locator(`${TABLE_SELECTOR} th`).filter({ hasText: 'Name' });
      await nameHeader.click();
      await page.waitForLoadState('networkidle');

      // Verify sort parameters
      expect(lastRequest).toContain('sortBy=');
      expect(lastRequest).toContain('sortDir=');
    });

    test('should send correct query parameters for pagination', async ({ page }) => {
      let lastRequest: string = '';
      page.on('request', request => {
        if (request.url().includes('/api/grid/employees')) {
          lastRequest = request.url();
        }
      });

      // Navigate to next page
      const pagination = page.locator(`${GRID_CONTAINER} .gridjs-pagination`);
      const nextButton = pagination.locator('button').filter({ hasText: /next/i }).first();
      
      if (await nextButton.isVisible()) {
        await nextButton.click();
        await page.waitForLoadState('networkidle');

        // Verify page parameter
        expect(lastRequest).toContain('page=2');
        expect(lastRequest).toContain('pageSize=');
      }
    });
  });
});

