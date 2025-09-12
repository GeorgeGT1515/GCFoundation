import { test, expect } from '@playwright/test';
import { runAxeAndAttach } from '../utils/a11y';

// Enable CSP bypass so axe can inject and external resources don't block
test.use({ bypassCSP: true });

function isSameOrigin(url: URL, base: URL) {
  return url.origin === base.origin;
}

function looksLikePagePath(pathname: string) {
  // Skip assets and files
  return !pathname.match(/\.(css|js|map|png|jpg|jpeg|gif|svg|ico|pdf|zip|xml)$/i);
}

function getCultureFromUrl(u: URL): string | undefined {
  const seg = u.pathname.split('/').filter(Boolean);
  if (seg.length > 0 && seg[0].length === 2) return seg[0].toLowerCase();
  return undefined; // enforce culture as first path segment only
}

function normalize(href: string, base: URL, allowedCultures: Set<string>): URL | undefined {
  try {
    const u = new URL(href, base);
    if (!isSameOrigin(u, base)) return undefined;
    if (!looksLikePagePath(u.pathname)) return undefined;
    u.hash = '';
    const culture = getCultureFromUrl(u);
    // Only follow links within allowed cultures; skip language chooser/root
    if (!culture || !allowedCultures.has(culture)) return undefined;
    return u;
  } catch {
    return undefined;
  }
}

test('a11y crawl across site @a11y @crawl', async ({ page }, testInfo) => {
  const baseURLStr = testInfo.project.use.baseURL as string;
  expect(baseURLStr).toBeTruthy();
  const base = new URL(baseURLStr);

  const maxPages = parseInt(process.env.CRAWL_MAX_PAGES || '30', 10);
  const culturesEnv = (process.env.CRAWL_CULTURES || 'en,fr').split(',').map(s => s.trim().toLowerCase()).filter(Boolean);
  const allowedCultures = new Set<string>(culturesEnv);

  // Seed by interacting with the language chooser to resolve true localized home routes
  const queue: URL[] = [];
  const cultureToLink: Record<string, RegExp> = { en: /English/i, fr: /Français/i };
  for (const c of allowedCultures) {
    const linkName = cultureToLink[c] ?? new RegExp(c, 'i');
    await page.context().clearCookies();
    await page.goto(base.toString());
    await page.waitForLoadState('load');
    await page.getByRole('link', { name: linkName }).click();
    await page.waitForLoadState('load');
    const current = new URL(page.url());
    if (getCultureFromUrl(current) === c) {
      queue.push(current);
    }
  }
  const visited = new Set<string>();
  const scanned: string[] = [];

  while (queue.length > 0 && scanned.length < maxPages) {
    const next = queue.shift()!;
    const key = next.toString();
    if (visited.has(key)) continue;
    visited.add(key);

    const res = await page.goto(next.toString());
    if (!res || !res.ok()) continue;
    await page.waitForLoadState('load');

    const results = await runAxeAndAttach(page, testInfo);
    const serious = results.violations.filter(v => v.impact === 'serious' || v.impact === 'critical');
    // soft-assert so crawl continues
    expect.soft(serious, `Serious/critical a11y violations on ${next.pathname}`).toHaveLength(0);
    scanned.push(next.pathname + next.search);

    // Discover links on this page
    const hrefs = await page.$$eval('a[href]', as => as.map(a => (a as HTMLAnchorElement).getAttribute('href') || ''));
    for (const href of hrefs) {
      const u = normalize(href, base, allowedCultures);
      if (!u) continue;
      const path = u.pathname.toLowerCase();
      // Exclude obvious non-page routes
      if (path.includes('logout')) continue;
      // Avoid language chooser/root
      if (path === '/' || path === '/language') continue;
      const ukey = u.toString();
      if (!visited.has(ukey)) queue.push(u);
    }
  }

  // Attach summary
  await testInfo.attach('crawl-summary.json', {
    body: JSON.stringify({ scannedCount: scanned.length, scanned }, null, 2),
    contentType: 'application/json'
  });
});


