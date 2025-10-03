import { test, expect } from '@playwright/test';

test.describe('Localization', () => {
  test('Language chooser routes to en/fr home @core', async ({ page }) => {
    // Landing at root should present language chooser or redirect based on cookie
    await page.context().clearCookies();
    await page.goto('/');
    await page.waitForLoadState('load');

    // Click English
    await page.getByRole('link', { name: /English/i }).click();
    await expect(page).toHaveURL(/\/home\?culture=en|\/home\b/);

    // Go back to language chooser and pick French
    await page.context().clearCookies();
    await page.goto('/');
    await page.getByRole('link', { name: /Français/i }).click();
    // Accept localized Home path too
    await expect(page).toHaveURL(/\/home\?culture=fr|\/home\b|\/fr\/accueil\b/);
  });
});


