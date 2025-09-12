## Initial E2E Test Catalog (Draft)

Tagging key: `@smoke`, `@core`, `@auth`, `@a11y`, `@navigation`.

### Smoke (PR quick suite)
- Home page loads and key header/footer elements visible (@smoke @core).
- Global nav renders and primary links navigate without error (@smoke @navigation).
- Static assets served without CSP violations (console checks) (@smoke).

### Core Components and Pages
- Localization toggles and persists between pages (if applicable) (@core).
- Error pages: 404 route shows custom page; 500 handler route shows friendly page (@core).
- Representative components from `GCFoundation.Components` render correctly in a demo page (@core).

### Authentication (if required)
- Login with valid test user leads to dashboard/protected page (@auth).
- Logout clears session; protected route redirects to login (@auth).
- Storage state reuse path for speeding up subsequent tests (@auth).

### Accessibility (in smoke)
- Axe scan on home and a key content page returns no serious violations (@a11y @smoke).

### Non-functional
- Console errors/warnings are captured and asserted minimal on key pages.
- Network request failures are detected (no 4xx/5xx for core assets/pages).

Notes
- This catalog will be refined after answering README’s open questions and after a brief app walk-through to identify the top critical user flows.


