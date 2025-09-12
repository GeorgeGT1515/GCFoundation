## Playwright Implementation Tasks (Phased Plan)

### Phase 0 – Foundations and Decisions
Status: Completed
- [x] Define environments and base URLs (Local only).
- [x] Auth strategy: None required now.
- [x] Approve repo layout for E2E: `e2e/` at root with TS.
- [ ] Confirm CI agents can install Chrome + Edge channels (pending first PR run)

Acceptance:
- [x] Questions in README answered; decisions documented.

### Phase 1 – Project Scaffolding
Status: Completed
- [x] Create `e2e/` folder with `package.json`, tsconfig, Playwright config.
- [x] Enable `@playwright/test` reporters: list + html + junit; artifacts retention.
- [x] Add scripts: `test:e2e`, `test:e2e:headed`, `test:e2e:ci`.
- [x] Configure baseURL via env var with fallback to local.

Acceptance:
- [x] `npm run test:e2e` runs a sample test successfully against local.

### Phase 2 – Environment & Data Management
Status: In Progress
Notes:
- Env handling via `PLAYWRIGHT_BASE_URL` is implemented. `.env.example` creation is optional (was blocked by ignore locally).
- [x] Introduce `.env` for local; document `.env.example`.
- [x] Add environment resolution (`PLAYWRIGHT_BASE_URL`).
- [x] No database; seed/reset not required.
- [x] Implement per-test isolation (stateless tests, clear cookies before locale switch tests).

Acceptance:
- [ ] Tests are independent and can run in parallel; no cross-test pollution.

### Phase 3 – CI Integration (PR Validation)
Status: Completed (pending first pipeline run verification)
Notes:
- PR pipeline updated to start `GCFoundation.Web` locally, install Chrome+Edge, run e2e, and publish artifacts.
- [x] Add Azure DevOps job to install Node and Playwright browsers (Chrome + Edge).
- [x] Start `GCFoundation.Web` in background for PR job (local target).
- [x] Run quick suite on PR using Chrome and Edge.
- [x] Publish HTML report, traces, and videos as artifacts.

Acceptance:
- [ ] PR pipeline shows E2E step, artifacts downloadable, job completes within budget.

### Phase 4 – Test Coverage of Critical Flows
Status: Completed
Notes:
- Initial smoke test (home + a11y + console filter) implemented. Navigation/components, localization, and error pages pending.
- Implement smoke suite:
  - [x] Home page
  - [x] Global navigation
  - [x] Key components render
- [x] Add localization checks (English/French selection).
- [x] Add auth flow tests (N/A — no auth for this app).
- [x] Add error page/edge case validation (404, 500 handler).
- Include axe a11y checks in smoke:
  - [x] Home page
  - [x] Key content pages (components index, card, template)

Acceptance:
- [ ] Smoke suite stable over 10 consecutive CI runs; flake rate <2%. (pending CI burn-in)

### Phase 5 – Browser Matrix & Schedules
Status: Partially Completed
Notes:
- PR runs Chrome + Edge. Nightly schedule and sharding not yet added.
- [x] PR: Chrome + Edge quick suite.
- [x] Nightly: Chrome + Edge full suite (add Firefox later if desired).
- [x] Configure retries for CI.
- [x] Shard tests (2 shards sequential on agent; can parallelize later).
- [x] Add nightly schedule via cron.

Acceptance:
- Nightly pipeline artifacts include cross-browser reports; no chronic flakes.

### Phase 6 – Quality & Observability
Status: Partially Completed
Notes:
- Axe included in smoke. Traces/videos/screenshots configured on failure. Tagging via `@smoke` in use. HAR and additional logging optional.
- [x] Integrate Axe for basic a11y checks.
- Quality artifacts:
  - [x] Screenshots on failure
  - [x] Trace on first retry
  - [x] Video on failure (CI)
  - [ ] Network HAR capture
  - [x] Console logs capture (test-level)
- [x] Attach detailed axe results (JSON + Markdown) to report
- [x] Tag tests by feature for targeted runs (`@smoke`, etc.).

Acceptance:
- [x] Reports are actionable; failures include trace, video, screenshot, console output.

### Phase 7 – Hardening & Docs
Status: Partially Completed
Notes:
- [x] Planning docs created.
- [ ] Finalize contributor docs for local setup and troubleshooting.
- [ ] Define test data contracts and SLAs for CI stability.
- [x] Add linting (`eslint`, `prettier`) to the `e2e/` project.

Acceptance:
- [ ] New devs can onboard and run E2E in <15 minutes with docs alone.

