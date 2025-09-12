## Local Environment & Developer Workflow

### Prerequisites
- Node.js 22.x (aligns with repo pipelines) or LTS 20.x if preferred.
- .NET SDK 8.x to run `GCFoundation.Web` locally when testing against localhost.

### Project Layout (proposed)
- `e2e/` at repository root
  - `package.json`, `playwright.config.ts`, `tsconfig.json`
  - `tests/` with spec files
  - `fixtures/`, `utils/`, `pages/` (Page Object Model optional)
  - `test-artifacts/` for traces/videos/screenshots (CI published)

### Setup
1. Create a local `.env` in `e2e/` (you can also create `.env.example` and commit it):
   - `PLAYWRIGHT_BASE_URL=http://localhost:5215` (or `https://localhost:7160`)
   - `E2E_USERNAME=...` (if needed)
   - `E2E_PASSWORD=...` (if needed)
2. Install deps and browsers:
   - `npm ci`
   - `npx playwright install chrome`
   - `npx playwright install msedge`

### Run the web app locally (if targeting localhost)
- `dotnet run --project GCFoundation.Web/GCFoundation.Web.csproj --urls https://localhost:7160;http://localhost:5215`

### Scripts (to be added in `e2e/package.json`)
- `test:e2e`: Run tests headless on Chrome + Edge.
- `test:e2e:headed`: Run headed for debugging.
- `test:e2e:ci`: Headless, retries=1, Chrome + Edge, traces on failure, JUnit + HTML.
- `test:smoke`: Minimal, fast checks for deployment validation.

### Good Practices
- Prefer `page.getByRole`/`getByLabel` over brittle CSS selectors.
- Use test IDs via `data-testid` only when semantic roles aren’t sufficient.
- Avoid sleeps; use `expect(locator).toBeVisible()` with timeouts.
- Keep tests independent; no assumptions about state from previous tests.
- Centralize URLs, credentials, and feature flags in config/env handling.
- No auth currently; keep tests stateless and independent.

### Troubleshooting
- If SSL issues on localhost, use the HTTP binding `http://localhost:5215`.
- For flakiness, enable traces/videos (`--trace on-first-retry`).
- Verify the app is ready: add a `/health` endpoint if not present and wait for it in tests.
- npm ci error (EUSAGE): create a lockfile first in `e2e/`:
  - `npm install --package-lock-only`
  - then `npm ci`

