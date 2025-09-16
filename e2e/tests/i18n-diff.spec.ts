import { test, expect } from '@playwright/test';
import allowlistConfig from '../i18n-allowlist.json';

// Same route list as a11y tests
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

// Allow-list of texts that may be legitimately identical across locales
const allowedIdenticalPatterns: RegExp[] = [
  /^gc\s*foundation$/i,
  /^gcfoundation$/i,
  /^canada$/i,
  /^api$/i,
  /^faq$/i,
  /^github$/i,
  /^html$/i,
  /^[A-Z]{2,6}$/,
  /^[0-9\-–—.,:%\s]+$/,
  /^https?:\/\//i,
  // Emails
  /^[\w.+-]+@[\w.-]+\.[A-Za-z]{2,}$/i,
  // Phone numbers
  /^\+?\d[\d\s().-]{6,}\d$/,
  // Numeric dates
  /^\d{4}-\d{2}-\d{2}$/,
  /^\d{1,2}[\/.\-]\d{1,2}[\/.\-]\d{2,4}$/,
  // Times
  /^\d{1,2}:\d{2}(?::\d{2})?(?:\s?[AP]M)?$/i,
  // Currency amounts
  /^[€$£]\s?\d{1,3}(?:[ ,. ]\d{3})*(?:[.,]\d{2})?$/,
  /^\d+(?:[.,]\d{2})?\s?(CAD|USD|EUR|GBP)$/i,
  // Postal/ZIP codes
  /^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z][ -]?\d[ABCEGHJ-NPRSTV-Z]\d$/i,
  /^\d{5}(?:-\d{4})?$/,
  // IPs
  /^\d{1,3}(?:\.\d{1,3}){3}$/,
  // Mixed IDs/codes
  /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d_-]{5,}$/
];

const externalExactNormalized = new Set<string>((allowlistConfig.exact ?? []).map(normalizeForCompare));
const externalRegexList: RegExp[] = (allowlistConfig.regex ?? []).map(p => new RegExp(p, 'i'));

function normalizeForCompare(input: string): string {
  return input
    .replace(/\s+/g, ' ')
    .trim()
    .toLowerCase()
    .replace(/[“”«»„”]/g, '"')
    .replace(/[’‘`´]/g, "'");
}

function isAllowedIdenticalNormalized(text: string): boolean {
  if (allowedIdenticalPatterns.some(rx => rx.test(text))) return true;
  if (externalExactNormalized.has(text)) return true;
  if (externalRegexList.some(rx => rx.test(text))) return true;
  return false;
}

async function collectVisibleTexts(page: import('@playwright/test').Page): Promise<string[]> {
  // Collect visible text nodes from the page, avoiding hidden elements and trivial strings
  const texts: string[] = await page.evaluate(() => {
    function isElementHidden(el: Element | null): boolean {
      if (!el || !(el instanceof HTMLElement)) return true;
      let cur: HTMLElement | null = el;
      while (cur) {
        const style = window.getComputedStyle(cur);
        if (style.display === 'none' || style.visibility === 'hidden' || cur.hidden) return true;
        cur = cur.parentElement;
      }
      const rect = (el as HTMLElement).getBoundingClientRect();
      return rect.width === 0 || rect.height === 0;
    }

    const excludedTags = new Set(['SCRIPT', 'STYLE', 'NOSCRIPT', 'SVG', 'CANVAS', 'IFRAME']);
    const results: string[] = [];
    const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT);
    let node: Node | null;
    let scanned = 0;
    const maxNodes = 8000;
    while ((node = walker.nextNode())) {
      if (++scanned > maxNodes) break;
      const parent = node.parentElement as HTMLElement | null;
      if (!parent || excludedTags.has(parent.tagName)) continue;
      if (parent.closest('[aria-hidden="true"], [hidden], [translate="no"], [data-i18n-ignore], .notranslate, .i18n-ignore, code')) continue;
      if (isElementHidden(parent)) continue;
      let raw = (node.nodeValue || '').replace(/\s+/g, ' ').trim();
      if (!raw) continue;
      if (raw.length < 3) continue;
      // Skip mostly punctuation or symbols
      const letters = raw.replace(/[^A-Za-zÀ-ÖØ-öø-ÿ]+/g, '');
      if (letters.length < 3) continue;
      results.push(raw);
    }
    return results;
  });
  return texts;
}

test.describe('I18n - content differs between English and French (detect hardcoded text)', () => {
  for (const route of routes) {
    test(`i18n-diff: ${route.name} @i18n`, async ({ page }, testInfo) => {
      // Collect EN
      await page.context().clearCookies();
      const enUrl = withCulture(route.path, 'en');
      const enRes = await page.goto(enUrl);
      expect(enRes?.ok(), `Failed to load EN ${enUrl}`).toBeTruthy();
      await page.waitForLoadState('load');
      const enTextsRaw = await collectVisibleTexts(page);

      // Collect FR
      await page.context().clearCookies();
      const frUrl = withCulture(route.path, 'fr');
      const frRes = await page.goto(frUrl);
      expect(frRes?.ok(), `Failed to load FR ${frUrl}`).toBeTruthy();
      await page.waitForLoadState('load');
      const frTextsRaw = await collectVisibleTexts(page);

      // Normalize and dedupe
      const enSet = new Map<string, string>();
      for (const t of enTextsRaw) {
        const n = normalizeForCompare(t);
        if (!enSet.has(n)) enSet.set(n, t);
      }
      const frSet = new Map<string, string>();
      for (const t of frTextsRaw) {
        const n = normalizeForCompare(t);
        if (!frSet.has(n)) frSet.set(n, t);
      }

      // Intersection of normalized strings that look like real phrases
      const identicalNormalized: string[] = [];
      for (const norm of enSet.keys()) {
        if (!frSet.has(norm)) continue;
        if (norm.length < 4) continue;
        if (!/[a-zà-öø-ÿ]/i.test(norm)) continue; // must contain letters
        if (isAllowedIdenticalNormalized(norm)) continue;
        identicalNormalized.push(norm);
      }

      // Heuristic thresholds
      const totalUniqueEn = enSet.size || 1;
      const ratio = identicalNormalized.length / totalUniqueEn;
      const ratioThreshold = 0.12; // 12% identical text is suspicious
      const absoluteThreshold = 12; // or too many identical strings

      // Build a concise sample list for the error and attachment
      const samples = identicalNormalized.slice(0, 30).map(n => ({
        text: n,
        example_en: enSet.get(n) || n,
        example_fr: frSet.get(n) || n
      }));

      const summary = {
        route: route.path,
        identicalCount: identicalNormalized.length,
        totalUniqueEn,
        ratio,
        samples
      };
      await testInfo.attach('i18n-diff-summary.json', {
        body: JSON.stringify(summary, null, 2),
        contentType: 'application/json'
      });

      const message = [
        `Identical EN/FR texts likely indicating hardcoded or untranslated content on ${route.name}.`,
        `Count: ${identicalNormalized.length} (ratio ${(ratio * 100).toFixed(1)}%).`,
        samples.length ? `Examples: ${samples.map(s => `"${s.text}"`).join(', ')}` : ''
      ].filter(Boolean).join(' ');

      expect(
        identicalNormalized.length <= absoluteThreshold && ratio <= ratioThreshold,
        message
      ).toBeTruthy();
    });
  }
});



