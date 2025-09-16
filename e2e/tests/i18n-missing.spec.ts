import { test, expect } from '@playwright/test';

// Reuse the same route list pattern as a11y tests
const routes: Array<{ path: string; name: string }> = [
  { path: '/home', name: 'Home' },
  { path: '/components', name: 'Components index' },
  { path: '/components/card', name: 'Components Card' },
  { path: '/components/badge', name: 'Components Badge' },
  { path: '/components/filtered-search', name: 'Components Filtered Search' },
  { path: '/components/form', name: 'Components Form' },
  { path: '/components/form-builder', name: 'Components Form Builder' },
  { path: '/components/modal', name: 'Components Modal' },
  { path: '/components/page-heading', name: 'Components Page Heading' },
  { path: '/components/stepper', name: 'Components Stepper' },
  { path: '/components/table', name: 'Components Table' },
  { path: '/components/user-login', name: 'Components User Login' },
  { path: '/template', name: 'Template' },
  { path: '/installation/global-resources', name: 'Global Resources' }
];

type Locale = 'en' | 'fr';
const locales: Locale[] = ['en', 'fr'];

function withCulture(path: string, locale: Locale): string {
  return path.includes('?') ? `${path}&culture=${locale}` : `${path}?culture=${locale}`;
}

function isConsoleMissing(text: string): boolean {
  const t = text.toLowerCase();
  return (
    /i18n.*missing/.test(t) ||
    /missing.*translation/.test(t) ||
    /missingkey/.test(t) ||
    /translation.*missing/.test(t) ||
    /i18n-missing/.test(t)
  );
}

function isMissingRequest(url: string, method: string): boolean {
  const u = url.toLowerCase();
  if (method.toUpperCase() !== 'POST') return false;
  // Common i18n endpoints for reporting missing keys (i18next and custom)
  return (
    u.includes('/locales/add') ||
    u.includes('/locales/missing') ||
    u.includes('report-missing') ||
    u.includes('savemissing') ||
    u.includes('/i18n/missing')
  );
}

test.describe('I18n - missing translations across pages', () => {
  for (const locale of locales) {
    for (const route of routes) {
      test(`i18n: ${route.name} (${locale}) @i18n`, async ({ page }, testInfo) => {
        const consoleHits: string[] = [];
        const requestHits: string[] = [];

        page.on('console', msg => {
          if (msg.type() === 'error' || msg.type() === 'warning' || msg.type() === 'debug') {
            const text = msg.text();
            if (isConsoleMissing(text)) consoleHits.push(text);
          }
        });

        page.on('request', req => {
          if (isMissingRequest(req.url(), req.method())) requestHits.push(`${req.method()} ${req.url()}`);
        });

        const target = withCulture(route.path, locale);
        const response = await page.goto(target);
        expect(response?.ok(), `Failed to load ${target}`).toBeTruthy();
        await page.waitForLoadState('load');

        // Allow any deferred i18n calls/logs to occur
        await page.waitForTimeout(100);

        // Scan DOM for obvious missing markers or keys leaked as text
        const domHints = await page.evaluate(() => {
          const hints: string[] = [];

          // 1) Explicit missing markers often used in dev setups
          const markerMatches = Array.from(document.querySelectorAll<HTMLElement>('body *'))
            .slice(0, 3000) // guard against extremely large pages
            .flatMap(el => {
              const txt = (el.textContent || '').trim();
              const hits: string[] = [];
              if (!txt) return hits;
              if (/\u27ea\u27ea?\s*missing[:\s]/i.test(txt)) hits.push(txt); // ⟪MISSING: ...⟫
              if (/\bmissing\s+translation\b/i.test(txt)) hits.push(txt);
              return hits;
            });
          hints.push(...markerMatches);

          // 2) Key-like tokens (e.g., namespace.key.other) that likely leaked to UI
          const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT);
          const keyRegex = /\b[a-z][a-z0-9_-]+(?:\.[a-z0-9_-]+){1,}\b/; // at least one dot segment
          const maxHints = 50;
          let n: Node | null;
          while ((n = walker.nextNode())) {
            const raw = (n.nodeValue || '').trim();
            if (!raw) continue;
            if (raw.length < 3 || raw.length > 120) continue;
            if (/[\s\u00A0]/.test(raw)) continue; // skip phrases
            if (!/[a-z]/i.test(raw)) continue; // skip pure numbers/punctuations
            if (keyRegex.test(raw)) {
              // Avoid common false positives: CSS classes, file names, urls
              if (/\.|\//.test(raw)) {
                if (/https?:\/\//i.test(raw)) continue;
                if (/\.(png|jpg|jpeg|gif|svg|css|js|map)$/i.test(raw)) continue;
              }
              hints.push(raw);
              if (hints.length >= maxHints) break;
            }
          }

          // 3) Elements explicitly marked as missing
          document.querySelectorAll('[data-i18n-missing]')
            .forEach(el => hints.push(`[attr] ${el.getAttribute('data-i18n-missing') || 'data-i18n-missing'}`));

          return Array.from(new Set(hints)).slice(0, 100);
        });

        const summary = {
          route: route.path,
          locale,
          consoleHits,
          requestHits,
          domHints
        };

        await testInfo.attach('i18n-summary.json', {
          body: JSON.stringify(summary, null, 2),
          contentType: 'application/json'
        });

        expect(consoleHits, 'No i18n-related console warnings/errors').toHaveLength(0);
        expect(requestHits, 'No i18n missing-key report requests').toHaveLength(0);
        expect(domHints, 'No obvious missing-translation markers or leaked i18n keys in DOM').toHaveLength(0);
      });
    }
  }
});



