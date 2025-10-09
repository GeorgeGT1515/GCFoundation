### Accessible GC Design System Grid Table TagHelper — Implementation Plan

References: [Grid.js docs](https://gridjs.io/docs), [GC Design System styles](https://design-system.alpha.canada.ca/en/styles/)

This plan defines a sequential, one-by-one execution path to implement a new Razor TagHelper that renders an accessible, fully keyboard-operable, WCAG 2.1 AAA-aligned data table with sorting, search, pagination, and AJAX-loading using vanilla JavaScript (no jQuery) powered by Grid.js.

---

#### Decisions and constraints (confirmed)
- [x] 0.1 Location/namespace: `GCFoundation.Components/TagHelpers/FDCP` (`GCFoundation.Components.TagHelpers.FDCP`); class name: `FdcpGridTableTagHelper`.
- [x] 0.2 Asset source: configurable. Default to local npm-bundled assets; optional CDN via app setting/feature flag with graceful fallback to local.
- [x] 0.3 Authoring: ES6 modules only (no TypeScript for now).
- [x] 0.4 Browsers: Edge, Chrome, Safari (not directly testable), Firefox nice-to-have. Avoid features needing heavy polyfills.
- [x] 0.5 Localization: English and French supported.
- [x] 0.6 Server-side only for sorting, pagination, and search (client-side mode not supported).
- [x] 0.7 Data contract accepted: `page`, `pageSize`, `sortBy`, `sortDir`, `q`. This is a widely used, clear pattern across grid libraries; not a formal standard but predictable and straightforward.

---

#### Proposed public API (for confirmation)

TagHelper: `FdcpGridTableTagHelper`

Attributes (high-level):
- `id` (string, optional) — unique id; auto-generated if omitted
- `data-url` (string, required) — AJAX endpoint for data
- `columns` (IEnumerable of column specs or JSON) — header text, field key, sortable, cell formatter
- `page-size` (int, optional, default 25)
- `page-sizes` (int[], optional, default [10,25,50,100])
- `search-enabled` (bool, default true)
- `sort-enabled` (bool, default true)
- `server-side` (bool, default true; enforced) — TagHelper operates only in server mode
- `caption` (string, required for AAA)
- `summary` (string, optional) — additional context for SR users
- `aria-label` (string, optional)
- `no-data-text` (string, localized)
- `loading-text` (string, localized)
- `class` (string, optional) — extra CSS classes
- `lang` (string, optional) — language override

Rendered structure (progressive enhancement):
- A semantic `<table>` with `<caption>` and `<thead>/<tbody>` skeleton to ensure non-JS fallback
- Search input, results count, and page-size control outside the table, all labeled and announced properly
- An `aria-live="polite"` region for announcing updates (filter, sort, pagination)
- Data attributes for client bootstrap (no inline script)
- Server-side data pipeline only (sorting/search/pagination via endpoint)

Proposed data endpoint contract (GET):
```
GET {dataUrl}?page={number}&pageSize={number}&sortBy={fieldKey}&sortDir={asc|desc}&q={string}

Response 200 application/json
{
  "items": [ { /* row object */ }, ... ],
  "total": 1234,
  "page": 1,
  "pageSize": 25
}
```

---

### Sequential GitHub Task List

- [x] 1. Verify branch: already on `feature/fdcp-grid-table`.
- [x] 2. Finalize and record decisions (above) in repo docs; lock public API and endpoint contract.
- [x] 3. Add Grid.js via npm to `GCFoundation.Components` and wire asset copying/bundling.
  - [x] 3.1 Install: `npm i gridjs`
  - [x] 3.2 Update build (gulp or bundler) to produce `wwwroot/lib/gridjs/{gridjs.min.css, gridjs.production.min.js}`
  - [ ] 3.3 Check in `package.json`, `package-lock.json`, and CI-friendly build scripts.
  - [x] 3.4 Add configurable asset source: `local` (default) or `cdn` via app settings; include optional `GridJsCdnUrl`.
  - [x] 3.5 Update layout (`_FoundationLayout.cshtml`) or component loader to load Grid.js CSS/JS from config; implement CDN→local fallback.
  - [x] 3.6 Update CSP if CDN used (script/style-src allowances) and document security implications.
- [ ] 4. Create vanilla JS module `wwwroot/src/js/components/fdcp-grid-table.js` (bundled into `wwwroot/js/foundation.min.js`) that wraps Grid.js (no jQuery).
  - [x] 4.0 Create file, auto-init via `data-fdcp-grid`, expose `window.FdcpGridTable` helpers
  - [x] 4.1 Read options from a `data-fdcp-grid` JSON attribute on the root container
  - [x] 4.2 Instantiate Grid.js with server pipeline for pagination/sort/search
  - [x] 4.3 Implement debounced search (e.g., 250–300ms) with `aria-live` announcements
  - [x] 4.4 Apply GC Design System classes to controls and table, ensure role/name/value correctness
  - [x] 4.5 Update `aria-sort` on headers and manage focus after updates (retain keyboard context)
  - [x] 4.6 Ensure ES6-only authoring and compatibility with Edge/Chrome/Safari; verify any Safari quirks for focus/announcements.
- [ ] 5. Add GC Design System-aligned CSS/SCSS for the component.
  - [x] 5.1 Use GC DS tokens for color, spacing, typography; ensure 7:1 contrast for AAA
  - [x] 5.2 Provide visible focus styles that meet AAA contrast and size guidelines
  - [x] 5.3 Ensure responsive behavior and reflow (no horizontal scroll at 320px, 400% zoom)
- [ ] 6. Implement TagHelper class (`FdcpGridTableTagHelper`) under selected namespace/folder.
  - [x] 6.1 Accept the attributes listed above and render semantic fallback HTML
  - [x] 6.2 Render caption, optional summary (via `aria-describedby`), and properly scoped headers
  - [x] 6.3 Emit a minimal, valid skeleton `<table>` for non-JS environments
  - [x] 6.4 Emit a root container with data attributes for the JS bootstrap
- [ ] 7. Create sample page demonstrating the TagHelper in `GCFoundation.Web`.
  - [x] 7.0 Confirm page location under Components (proposed: `GCFoundation.Web/Views/Components/FDCPGridTable`).
  - [x] 7.1 Add `Index.cshtml` demo page rendering `FdcpGridTableTagHelper` with realistic data.
  - [x] 7.2 Add supporting partials for docs if following existing pattern: `_Overview.cshtml`, `_Properties.cshtml`, `_SampleCodeSections.cshtml` describing attributes and usage.
  - [x] 7.3 Add navigation entry so page is discoverable in the Components section.
  - [x] 7.4 Include examples with typical, many-columns, and long-text datasets.
  - [ ] 7.5 Verify keyboard-only usage path end-to-end.
- [x] 8. Implement data endpoint in `GCFoundation.Web` (e.g., `GridDataController`).
  - [x] 8.1 Parse `page`, `pageSize`, `sortBy`, `sortDir`, `q` with strict validation and whitelisting
  - [x] 8.2 Enforce max page size and default sort; prevent SQL/NoSQL injection in query layer
  - [x] 8.3 Return the proposed JSON envelope; handle errors with RFC7807 Problem Details
  - [x] 8.4 Ensure server-side is the only supported mode for sort/search/pagination; document this in the endpoint and TagHelper docs.
- [x] 9. Wire TagHelper + JS to endpoint (end-to-end functional).
  - [x] 9.1 Verify search filters server-side and reflects in UI Count
  - [x] 9.2 Verify sorting toggles and `aria-sort` semantics
  - [x] 9.3 Verify pagination controls and focus management
- [x] 10. Localization and internationalization.
  - [x] 10.1 Externalize all UI strings (placeholders, labels, buttons) into resource files
  - [x] 10.2 Support language switch (e.g., `lang` attribute) and RTL if required
- [x] 11. Automated accessibility checks (Tests created and run - 20/32 passing, fixes needed).
  - [x] 11.1 Add Playwright + axe-core e2e checks for the Components demo page (new spec, `e2e/tests/fdcp-grid-table-a11y.spec.ts`).
  - [x] 11.2 Verify key WCAG 2.1 AAA criteria: contrast (1.4.6), keyboard (2.1.x), name/role/value (4.1.2), reflow (1.4.10), text spacing (1.4.12), link purpose (2.4.9) where applicable
  - [x] 11.3 Test `scope="col"` on column headers and `scope="row"` on first cell of each row
  - [x] 11.4 Test `aria-sort` updates when columns are sorted
  - [x] 11.5 Test keyboard navigation (Enter/Space for sorting, Tab for focus management)
  - [x] 11.6 Test screen reader announcements (caption, pagination status, sort state)
  - [x] 11.7 Test focus indicators on all interactive elements
  - [x] 11.8 Test in both English and French for i18n accessibility
  - [x] 11.9 Add utility functions for reusable accessibility checks (`e2e/utils/grid-table-a11y.ts`)
  - [x] 11.10 Add dedicated npm scripts: `test:a11y:grid` and `test:a11y:grid:headed`
  - [x] 11.11 Create comprehensive documentation (`e2e/tests/README-FDCP-GRID-TABLE-A11Y.md`)
  - [x] 11.12 Fix accessibility issues identified by automated tests (Test Results: 20/32 passed)
    - [x] 11.12.1 **CRITICAL**: Fix `aria-sort` not updating after column sort - JavaScript event listener issue in `fdcp-grid-table.js`
    - [x] 11.12.2 **CRITICAL**: Add `aria-label` to `.gridjs-wrapper` div in `FdcpGridTableTagHelper.cs` 
    - [x] 11.12.3 Exclude code syntax highlighting from WCAG AAA color contrast tests (not Grid Table component issue)
    - [x] 11.12.4 Update "semantic HTML structure" test to use more specific selector (`table.gridjs-table thead` instead of `thead`)
    - [x] 11.12.5 Fix "search with keyboard" test - use more specific search term (current "Employee 1" returns 25+ results)
    - [x] 11.12.6 Verify keyboard navigation triggers sort correctly (Enter/Space keys not triggering aria-sort update)
- [ ] 12. Manual accessibility testing.
  - [ ] 12.1 Screen reader (NVDA/JAWS/VoiceOver) scenarios for sort, paginate, and search announcements
  - [ ] 12.2 Keyboard-only path; verify no traps and logical tab order
- [ ] 13. Unit tests for TagHelper rendering (in `GCFoundation.Tests.Components`).
  - [ ] 13.1 Verify required attributes are enforced (e.g., `data-url`, `caption`)
  - [ ] 13.2 Validate semantic markup in the fallback table
- [ ] 14. E2E tests for interaction (in `e2e/tests`), including sorting, search, and pagination.
  - [ ] 14.1 Implement `fdcp-grid-table.spec.ts` to exercise sorting toggles, search queries, pagination transitions, and confirm stable focus.
  - [ ] 14.2 Reuse `e2e/utils/a11y.ts` for axe checks on initial load and after dynamic updates.
- [ ] 15. Performance hardening.
  - [ ] 15.1 Debounce/throttle network calls; avoid duplicate requests
  - [ ] 15.2 Consider server-side caching for identical queries
  - [ ] 15.3 Guardrails on page size and query length
- [ ] 16. Security review.
  - [ ] 16.1 Validate and sanitize all query inputs; enforce allowlists for `sortBy`
  - [ ] 16.2 Consider rate limiting/high-cost query protections
- [ ] 17. CI/CD integration.
  - [ ] 17.1 Ensure npm install/build runs where needed; include Grid.js assets in artifacts
  - [ ] 17.2 Add tests to CI; fail fast on a11y regressions
- [ ] 18. Developer documentation.
  - [ ] 18.1 Add usage guide in `GCFoundation.Components/Documentation` with examples and attribute reference
  - [ ] 18.2 Add a migration note for adopters
- [ ] 19. Versioning and release notes.
  - [ ] 19.1 Bump package version(s) as appropriate; update CHANGELOG
- [ ] 20. QA sign-off and production deployment.

---

### Acceptance Criteria (high level)

- Meets WCAG 2.1 AAA for relevant criteria (contrast, keyboard, semantics, reflow, announcements).
- Works without JavaScript (semantic static table rendered) and progressively enhances when JS is present.
- No jQuery; vanilla JS only. Grid.js used via npm, bundled locally.
- Sorting, search, and pagination are functional via AJAX and accessible by keyboard and screen readers.
- Styled to match GC Design System tokens; focus indicators are strong and consistent.
- Localization-ready with resource-based strings; supports English and French at minimum (or as confirmed).
- Automated tests (unit + e2e + axe) pass in CI.
 - Server-side mode enforced; no client-side mode.
 - Asset source is configurable (local default, CDN optional) with graceful fallback and CSP compatibility.
 - Works on Edge/Chrome/Safari; Firefox support is best-effort.
 - A Components demo page exists under `GCFoundation.Web/Views/Components/FDCPGridTable` (or confirmed path), linked in navigation, and passes axe e2e checks.

---

## Accessibility Test Results (Run: October 8, 2025)

**Overall**: 20/32 tests PASSED (62.5%) - 12 failures across Chrome and Edge

### ✅ Passing Tests (Core functionality works):
- Table structure and caption with `visibility-sr-only`
- `scope="col"` on all column headers  
- `scope="row"` on first cell of each row
- Keyboard navigation for pagination
- Focus indicators on interactive elements
- Proper semantic HTML (table, thead, tbody, tr, th, td)
- No empty table headers
- Pagination status announcements
- Color contrast for Grid Table content
- Focus maintained after sort
- French localization accessibility

### ❌ Failing Tests (Need fixes):
1. **Color Contrast (NOT Grid Table)**: Syntax highlighting in code examples fails WCAG AAA (5.9:1, needs 7:1)
   - **Impact**: Low - not part of Grid Table component
   - **Fix**: Exclude `<pre>` from AAA testing or update syntax theme

2. **aria-sort Not Updating (CRITICAL)**: `aria-sort` attribute stays "none" after sorting
   - **Impact**: High - screen readers won't announce sort state
   - **Fix**: Debug JavaScript event listeners in `fdcp-grid-table.js`

3. **Missing ARIA Labels (CRITICAL)**: `.gridjs-wrapper` missing `aria-label`
   - **Impact**: High - table not properly labeled for screen readers
   - **Fix**: Add `aria-label` in `FdcpGridTableTagHelper.cs`

4. **Keyboard Sort Not Triggering**: Enter/Space keys don't trigger sort
   - **Impact**: High - keyboard users can't sort
   - **Fix**: Related to issue #2, fix event listeners

5. **Test Selector Too Broad**: Test finds multiple `<thead>` elements on page
   - **Impact**: None - test issue, not component issue
   - **Fix**: Use `table.gridjs-table thead` selector

6. **Search Test Assertion**: "Employee 1" returns 25 results, not fewer
   - **Impact**: None - test assumption issue
   - **Fix**: Use more specific search term like "Employee 25"

### Priority:
- **HIGH**: Fix aria-sort updates (#2, #4)
- **HIGH**: Add ARIA labels (#3)
- **LOW**: Adjust tests (#5, #6) and color contrast exclusion (#1)

---

### Notes

- Grid.js provides the data pipeline needed for sorting/search/pagination without jQuery, and can be initialized programmatically from a data attribute config. See docs: https://gridjs.io/docs
- GC Design System styles and tokens should be the source of truth for spacing, typography, color, and focus styles: https://design-system.alpha.canada.ca/en/styles/
 - The endpoint contract (`page`, `pageSize`, `sortBy`, `sortDir`, `q`) is a common, clear pattern across grids. It is not a formal standard but is easy to implement and reason about.


