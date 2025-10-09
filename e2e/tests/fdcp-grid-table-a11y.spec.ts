import { test, expect } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';
import { runAxeAndAttach } from '../utils/a11y';

/**
 * Comprehensive accessibility tests for FDCP Grid Table component
 * Tests WCAG 2.1 AAA compliance, ARIA attributes, keyboard navigation, and screen reader support
 */

test.describe('FDCP Grid Table - Accessibility @a11y', () => {
  // Allow axe injection and external resources
  test.use({ bypassCSP: true });

  test.beforeEach(async ({ page }) => {
    await page.goto('/components/fdcp-grid-table');
    // Wait for Grid.js to initialize
    await page.waitForSelector('.gridjs-wrapper', { state: 'visible', timeout: 10000 });
    await page.waitForTimeout(500); // Extra buffer for dynamic rendering
  });

  test('should pass axe WCAG AAA checks', async ({ page }, testInfo) => {
    // Run axe with WCAG 2.1 AAA tags
    // Exclude code syntax highlighting (not part of Grid Table component)
    const results = await new AxeBuilder({ page })
      .withTags(['wcag2a', 'wcag2aa', 'wcag2aaa'])
      .exclude('pre') // Exclude code blocks from color contrast checks
      .analyze();

    // Attach results manually since we're not using runAxeAndAttach
    await testInfo.attach('axe-results.json', { 
      body: JSON.stringify(results, null, 2), 
      contentType: 'application/json' 
    });
    
    const serious = results.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serious, 'No serious/critical WCAG AAA violations').toHaveLength(0);
  });

  test('should have proper table structure and caption', async ({ page }) => {
    // Check for table element
    const table = page.locator('table.gridjs-table');
    await expect(table).toBeVisible();

    // Check for caption with visibility-sr-only class (screen reader only)
    const caption = table.locator('caption');
    await expect(caption).toBeVisible(); // toBeVisible checks if element exists in DOM
    
    const captionClass = await caption.getAttribute('class');
    expect(captionClass).toContain('visibility-sr-only');
    
    const captionText = await caption.textContent();
    expect(captionText?.trim()).toBeTruthy();
    expect(captionText?.trim().length).toBeGreaterThan(0);
  });

  test('should have scope="col" on all column headers', async ({ page }) => {
    const headers = page.locator('thead th.gridjs-th');
    const count = await headers.count();
    expect(count).toBeGreaterThan(0);

    // Check each header has scope="col"
    for (let i = 0; i < count; i++) {
      const header = headers.nth(i);
      const scope = await header.getAttribute('scope');
      expect(scope, `Header ${i} should have scope="col"`).toBe('col');
    }
  });

  test('should have scope="row" on first cell of each row', async ({ page }) => {
    const rows = page.locator('tbody tr.gridjs-tr');
    const rowCount = await rows.count();
    expect(rowCount).toBeGreaterThan(0);

    // Check first cell of each row has scope="row"
    for (let i = 0; i < Math.min(rowCount, 5); i++) { // Check first 5 rows
      const firstCell = rows.nth(i).locator('td').first();
      const scope = await firstCell.getAttribute('scope');
      expect(scope, `First cell of row ${i} should have scope="row"`).toBe('row');
    }
  });

  test('should update aria-sort when column is sorted', async ({ page }) => {
    // Get first sortable header
    const firstHeader = page.locator('thead th.gridjs-th').first();
    await expect(firstHeader).toBeVisible();

    // Initial state - should have aria-sort="none" or no aria-sort
    let ariaSort = await firstHeader.getAttribute('aria-sort');
    expect(ariaSort === 'none' || ariaSort === null).toBeTruthy();

    // Click to sort ascending
    await firstHeader.click();
    await page.waitForTimeout(500); // Wait for sort to complete

    // Should now have aria-sort="ascending"
    ariaSort = await firstHeader.getAttribute('aria-sort');
    expect(ariaSort).toBe('ascending');

    // Click again to sort descending
    await firstHeader.click();
    await page.waitForTimeout(500);

    // Should now have aria-sort="descending"
    ariaSort = await firstHeader.getAttribute('aria-sort');
    expect(ariaSort).toBe('descending');

    // Other headers should have aria-sort="none"
    const secondHeader = page.locator('thead th.gridjs-th').nth(1);
    const secondAriaSort = await secondHeader.getAttribute('aria-sort');
    expect(secondAriaSort).toBe('none');
  });

  test('should have proper ARIA labels', async ({ page }) => {
    // Check table container has aria-label or aria-labelledby
    const wrapper = page.locator('.gridjs-wrapper');
    const ariaLabel = await wrapper.getAttribute('aria-label');
    const ariaLabelledBy = await wrapper.getAttribute('aria-labelledby');
    
    expect(ariaLabel !== null || ariaLabelledBy !== null, 
      'Table should have aria-label or aria-labelledby').toBeTruthy();

    // Check search input has label
    const searchInput = page.locator('.gridjs-search input');
    if (await searchInput.count() > 0) {
      const searchAriaLabel = await searchInput.getAttribute('aria-label');
      const searchPlaceholder = await searchInput.getAttribute('placeholder');
      expect(searchAriaLabel !== null || searchPlaceholder !== null,
        'Search input should have aria-label or placeholder').toBeTruthy();
    }
  });

  test('should support keyboard navigation for sorting', async ({ page }) => {
    const firstHeader = page.locator('thead th.gridjs-th').first();
    const firstHeaderButton = firstHeader.locator('button');
    
    // Focus the inner sort button (stable across re-renders)
    await firstHeaderButton.waitFor({ state: 'visible' });
    await firstHeaderButton.focus();
    
    // Verify it's focused
    const isFocused = await firstHeaderButton.evaluate(el => el === document.activeElement);
    expect(isFocused).toBeTruthy();

    // Press Enter to sort - wait for network response
    const [response] = await Promise.all([
      page.waitForResponse(
        response => {
          const matches = response.url().includes('/api/grid/employees') && response.status() === 200;
          return matches;
        },
        { timeout: 5000 }
      ),
      firstHeaderButton.press('Enter')
    ]);
    
    await page.waitForTimeout(1000); // Extra time for Grid.js to update DOM and aria-sort

    // Verify aria-sort changed
    const ariaSort = await firstHeader.getAttribute('aria-sort');
    expect(ariaSort).toBe('ascending');

    // Re-query and re-focus the inner sort button after DOM update (important!)
    const firstHeaderAfterSort = page.locator('thead th.gridjs-th').first();
    const firstHeaderButtonAfterSort = firstHeaderAfterSort.locator('button');
    await firstHeaderButtonAfterSort.waitFor({ state: 'visible' });
    await firstHeaderButtonAfterSort.focus();
    
    // Verify it's focused again
    const isFocusedAgain = await firstHeaderButtonAfterSort.evaluate(el => el === document.activeElement);
    expect(isFocusedAgain).toBeTruthy();

    // Press Space to sort again - wait for network response
    const [response2] = await Promise.all([
      page.waitForResponse(
        response => response.url().includes('/api/grid/employees') && response.status() === 200,
        { timeout: 5000 }
      ),
      firstHeaderButtonAfterSort.press('Space')
    ]);
    
    await page.waitForTimeout(1000); // Extra time for Grid.js to update DOM and aria-sort

    // Verify aria-sort changed to descending
    const ariaSortAfter = await firstHeaderAfterSort.getAttribute('aria-sort');
    expect(ariaSortAfter).toBe('descending');
  });

  test('should support keyboard navigation for pagination', async ({ page }) => {
    // Find pagination buttons
    const nextButton = page.locator('.gridjs-pages button').filter({ hasText: 'Next' });
    
    if (await nextButton.count() > 0) {
      // Tab to pagination area
      await nextButton.focus();
      
      // Verify focus
      const isFocused = await nextButton.evaluate(el => el === document.activeElement);
      expect(isFocused).toBeTruthy();

      // Press Enter to go to next page
      await page.keyboard.press('Enter');
      await page.waitForTimeout(500);

      // Verify page changed
      const summary = page.locator('.gridjs-summary');
      const summaryText = await summary.textContent();
      expect(summaryText).toContain('26'); // Should show "Showing 26 to..."
    }
  });

  test('should have proper focus indicators', async ({ page }) => {
    const firstHeader = page.locator('thead th.gridjs-th').first();
    await firstHeader.focus();

    // Check computed outline or box-shadow (focus indicator)
    const outline = await firstHeader.evaluate(el => {
      const styles = window.getComputedStyle(el);
      return {
        outline: styles.outline,
        outlineWidth: styles.outlineWidth,
        outlineStyle: styles.outlineStyle,
        boxShadow: styles.boxShadow
      };
    });

    // Should have some form of focus indicator
    const hasFocusIndicator = 
      outline.outlineWidth !== '0px' || 
      outline.boxShadow !== 'none';
    
    expect(hasFocusIndicator, 'Header should have visible focus indicator').toBeTruthy();
  });

  test('should have proper semantic HTML structure', async ({ page }) => {
    // Verify table has thead and tbody - use specific selector for Grid Table
    const thead = page.locator('table.gridjs-table thead');
    const tbody = page.locator('table.gridjs-table tbody');
    
    await expect(thead).toBeVisible();
    await expect(tbody).toBeVisible();

    // Verify rows use tr elements
    const theadRows = thead.locator('tr');
    await expect(theadRows.first()).toBeVisible();

    const tbodyRows = tbody.locator('tr');
    const rowCount = await tbodyRows.count();
    expect(rowCount).toBeGreaterThan(0);

    // Verify headers use th elements
    const thElements = thead.locator('th');
    const thCount = await thElements.count();
    expect(thCount).toBeGreaterThan(0);

    // Verify body cells use td elements
    const tdElements = tbody.locator('td');
    const tdCount = await tdElements.count();
    expect(tdCount).toBeGreaterThan(0);
  });

  test('should announce pagination status to screen readers', async ({ page }) => {
    // Check for live region or status role on pagination summary
    const summary = page.locator('.gridjs-summary');
    await expect(summary).toBeVisible();

    const role = await summary.getAttribute('role');
    const ariaLive = await summary.getAttribute('aria-live');
    const ariaAtomic = await summary.getAttribute('aria-atomic');

    expect(role).toBe('status');
    // Note: aria-live and aria-atomic might be set by Grid.js or our code
  });

  test('should have no empty table headers', async ({ page }) => {
    const headers = page.locator('thead th.gridjs-th');
    const count = await headers.count();

    for (let i = 0; i < count; i++) {
      const headerText = await headers.nth(i).textContent();
      expect(headerText?.trim().length, `Header ${i} should not be empty`).toBeGreaterThan(0);
    }
  });

  test('should support search with keyboard', async ({ page }) => {
    const searchInput = page.locator('.gridjs-search input');
    
    if (await searchInput.count() > 0) {
      // Focus and type in search - use more specific term
      await searchInput.focus();
      await searchInput.fill('Employee 25');
      await page.waitForTimeout(1500); // Wait for search debounce and server response

      // Verify results filtered
      const rows = page.locator('tbody tr.gridjs-tr');
      const rowCount = await rows.count();
      
      // Should have fewer rows after filtering (Employee 25, 125, 225, etc.)
      expect(rowCount).toBeGreaterThan(0);
      expect(rowCount).toBeLessThanOrEqual(10); // Should be much fewer than 25

      // Clear search
      await searchInput.fill('');
      await page.waitForTimeout(1000);

      // Should restore all rows
      const rowCountAfter = await rows.count();
      expect(rowCountAfter).toBe(25); // Back to page size
    }
  });

  test('should maintain focus after sort', async ({ page }) => {
    const firstHeader = page.locator('thead th.gridjs-th').first();
    
    await firstHeader.focus();
    await firstHeader.click();
    await page.waitForTimeout(500);

    // Focus should remain on the header after sort
    const stillFocused = await firstHeader.evaluate(el => el === document.activeElement);
    expect(stillFocused, 'Header should maintain focus after sort').toBeTruthy();
  });

  test('should have proper color contrast for text', async ({ page }) => {
    // This is a basic check - axe will do more comprehensive contrast checks
    const cell = page.locator('tbody td.gridjs-td').first();
    await expect(cell).toBeVisible();

    const colors = await cell.evaluate(el => {
      const styles = window.getComputedStyle(el);
      return {
        color: styles.color,
        backgroundColor: styles.backgroundColor
      };
    });

    // Basic validation that colors are set
    expect(colors.color).toBeTruthy();
    expect(colors.backgroundColor).toBeTruthy();
  });

  test('should localize in French when language changes', async ({ page }) => {
    // Navigate to French version
    await page.goto('/fr/composants/fdcp-grid-table');
    await page.waitForSelector('.gridjs-wrapper', { state: 'visible', timeout: 10000 });
    await page.waitForTimeout(500);

    // Check that caption is in French
    const caption = page.locator('table.gridjs-table caption');
    const captionText = await caption.textContent();
    expect(captionText).toContain('Employés'); // French for "Employees"

    // Check that headers are in French
    const headers = page.locator('thead th.gridjs-th');
    const headerTexts = await headers.allTextContents();
    
    // Should contain French text like "Nom" or "Département"
    const hasFrenchText = headerTexts.some(text => 
      text.includes('Nom') || text.includes('Département')
    );
    expect(hasFrenchText, 'Headers should be in French').toBeTruthy();

    // Run axe on French version
    const results = await runAxeAndAttach(page, test.info(), ['wcag2a', 'wcag2aa']);
    const serious = results.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    expect(serious, 'No serious violations in French version').toHaveLength(0);
  });
});

