import { test, expect } from '@playwright/test';

test.describe('Components Demo', () => {
  test('Card component page renders demo markup @core', async ({ page }) => {
    await page.goto('/components/card');
    await page.waitForLoadState('load');

    // Validate demo title without depending on custom element visibility
    const title = page.locator('.fdcp-card-title');
    await expect(title.first()).toBeVisible();
    await expect(title.first()).toHaveText(/Card title/i);
  });
});


