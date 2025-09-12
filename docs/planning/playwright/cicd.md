## Azure DevOps CI/CD Integration for Playwright

This plan integrates Playwright runs into existing YAML pipelines:

### 1) PR Validation (`pr-validation-pipeline.yml`)
- Install Node (22.x to match repo usage) and Playwright browsers (Chrome + Edge channels).
- Spin up `GCFoundation.Web` locally in the agent:
  - `dotnet build GCFoundation.Web/GCFoundation.Web.csproj -c Release`
  - `dotnet run --project GCFoundation.Web/GCFoundation.Web.csproj --urls http://localhost:5215 &`
  - Wait-for-HTTP root `http://localhost:5215/` (no `/health` endpoint yet).
- Run quick suite with Chrome and Edge, retries=1.
- Publish artifacts: HTML report, traces, videos.
- Target duration: ~3–5 minutes for PR suite; keep tests focused and parallelized.

YAML snippet (to add under Test stage):

```yaml
- task: UseNode@1
  displayName: Install Node.js
  inputs:
    version: '22.x'

- script: |
    npm ci
    npx playwright install --with-deps chrome
    npx playwright install --with-deps msedge
  displayName: Install Playwright and Chrome/Edge
  workingDirectory: e2e

- task: DotNetCoreCLI@2
  displayName: Start Web App (background)
  inputs:
    command: 'run'
    projects: 'GCFoundation.Web/GCFoundation.Web.csproj'
    arguments: '--urls http://localhost:5215'
  continueOnError: true

- powershell: |
    $ErrorActionPreference = 'Stop'
    $url = 'http://localhost:5215/'
    for ($i=0; $i -lt 60; $i++) {
      try { (Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 5) | Out-Null; break }
      catch { Start-Sleep -Seconds 2 }
    }
  displayName: Wait for site to be ready

- script: |
    set PLAYWRIGHT_BASE_URL=http://localhost:5215
    npm run test:e2e:ci
  displayName: Run Playwright (PR quick suite: Chrome + Edge)
  workingDirectory: e2e

- task: PublishBuildArtifacts@1
  displayName: Publish Playwright Report
  inputs:
    pathToPublish: 'e2e/playwright-report'
    artifactName: 'playwright-report'

- task: PublishBuildArtifacts@1
  displayName: Publish Playwright Traces & Videos
  inputs:
    pathToPublish: 'e2e/test-artifacts'
    artifactName: 'playwright-artifacts'
```

Notes:
- Configure `PLAYWRIGHT_BASE_URL` as pipeline variable to switch between Dev URL and local.
- Prefer Chromium-only for PR to keep time budget low.

### 2) Web Build/Deploy Pipeline (`Pipeline/gcfoundationweb-azure-pipelines.yaml`)
- Add optional post-deploy smoke job targeting the newly deployed slot/site.
- Fail fast on smoke to catch bad deploys early.

YAML sketch:

```yaml
- stage: e2e_smoke
  dependsOn: deploy_dev
  condition: and(succeeded(), ne(variables['Build.Reason'], 'PullRequest'))
  jobs:
  - job: run_smoke
    steps:
    - task: UseNode@1
      inputs:
        version: '22.x'
    - script: |
        npm ci
        npx playwright install --with-deps chromium
      workingDirectory: e2e
    - script: |
        set PLAYWRIGHT_BASE_URL=$(devWebAppUrl)
        npm run test:smoke
      workingDirectory: e2e
    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: 'e2e/playwright-report'
        artifactName: 'playwright-report-dev'
```

### 3) Nightly Cross-Browser
- Schedule a pipeline that runs Chrome + Edge (full suite). Optionally add Firefox later.
- Shard tests across agents for speed; set retries=2.

### Artifact & Reporting Standards
- Always publish: `playwright-report/`, `test-artifacts/` (traces, videos, screenshots), JUnit XML.
- Keep last N builds’ artifacts to manage storage.

### Security & Secrets
- Store credentials in Azure DevOps Library variable groups; map to env vars at runtime.
- Do not commit secrets. Provide `.env.example` for local use only.

### Flake Management
- Use `expect.poll`, `locator`-based assertions, and deterministic test data.
- Retry only on CI; capture traces on first retry for diagnostics.

