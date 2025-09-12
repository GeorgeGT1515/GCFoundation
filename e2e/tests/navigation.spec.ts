import { test, expect } from '@playwright/test';

test.describe('Navigation', () => {
  test('top-level pages load @smoke @navigation', async ({ page }) => {
    await page.goto('/home');
    await page.waitForLoadState('load');
    await expect(page).toHaveURL(/\/home/i);

    await page.goto('/components');
    await page.waitForLoadState('load');
    await expect(page).toHaveURL(/\/components/i);

    await page.goto('/template');
    await page.waitForLoadState('load');
    await expect(page).toHaveURL(/\/template/i);
  });
});


