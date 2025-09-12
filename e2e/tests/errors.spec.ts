import { test, expect } from '@playwright/test';

test.describe('Error Pages', () => {
  test('NotFound error page sets 404 status and renders content @core', async ({ page }) => {
    const res = await page.goto('/Error/NotFound');
    expect(res?.status()).toBe(404);
    await page.waitForLoadState('load');
    // Basic presence of content
    await expect(page.locator('text=Not Found')).toHaveCount(1);
  });

  test('Global error page renders content @core', async ({ page }) => {
    const res = await page.goto('/Error/Global');
    expect(res?.ok()).toBeTruthy();
    await page.waitForLoadState('load');
    await expect(page.locator('text=Error')).toHaveCount(1);
  });
});


