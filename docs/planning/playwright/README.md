## Playwright Adoption Plan – Overview

**Goals**
- Establish reliable end-to-end (E2E) testing for `GCFoundation.Web` with Playwright.
- Integrate E2E into Azure DevOps PR validation and release pipelines.
- Provide fast local developer workflow, stable CI runs, and actionable reports (HTML + trace + video).
- Follow good practices: test isolation, deterministic data, idempotent setup/teardown, and security-conscious handling of secrets.

**Scope**
- Primary target: ASP.NET Core app `GCFoundation.Web` (net8.0). 
- Browsers: Chrome and Edge channels (Chromium-based).
- Environments: Local first (https://localhost:7160 / http://localhost:5215). Dev/QA/UAT later.

**Confirmed Decisions**
- No authentication required.
- Start with local-only PR runs against a locally started `GCFoundation.Web`.
- No secrets required for this app at this time.
- Browsers on PR: Chrome and Edge channels.
- Project location: `e2e/` at repo root; Language: TypeScript; Runner: `@playwright/test`.

**Success Criteria**
- Developers can run `npm run test:e2e` locally and see stable results in <5 minutes.
- CI runs Playwright on PRs with artifacts (HTML report, traces, videos) published.
- Critical flows covered (navigation, localization, auth/session behavior if applicable, core components render).
- Flake rate <2% over 2 consecutive weeks.
 - Smoke suite includes basic axe-based accessibility checks.

**Open Questions (remaining)**
1) PR time budget: we will target 3–5 minutes; confirm acceptable if suite grows.
2) Health readiness: We will wait on root URL; add `/health` endpoint later?

Next: see `tasks.md`, `cicd.md`, `environment.md`, and `catalog.md` in this folder.

