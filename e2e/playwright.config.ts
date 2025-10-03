import { defineConfig, devices } from '@playwright/test';
import * as dotenv from 'dotenv';

dotenv.config();

const baseURL = process.env.PLAYWRIGHT_BASE_URL || 'http://localhost:5215';

// Allow CI to restrict projects via env var, e.g. PLAYWRIGHT_PROJECTS=chrome
const selectedProjectsRaw = process.env.PLAYWRIGHT_PROJECTS;
const selectedProjects = selectedProjectsRaw
  ? selectedProjectsRaw.split(',').map(p => p.trim().toLowerCase()).filter(Boolean)
  : null;

const projectDefinitions = [
  {
    name: 'chrome',
    use: { ...devices['Desktop Chrome'], channel: 'chrome' }
  },
  {
    name: 'edge',
    use: { ...devices['Desktop Edge'], channel: 'msedge' }
  }
];

const projects = selectedProjects && selectedProjects.length > 0
  ? projectDefinitions.filter(p => selectedProjects.includes(p.name.toLowerCase()))
  : projectDefinitions;

export default defineConfig({
  testDir: './tests',
  timeout: 30_000,
  expect: { timeout: 10_000 },
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: process.env.CI ? undefined : undefined,
  reporter: [['list'], ['html', { open: 'never', outputFolder: 'playwright-report' }], ['junit', { outputFile: 'test-results/junit.xml' }]],
  use: {
    baseURL,
    trace: process.env.CI ? 'on-first-retry' : 'retain-on-failure',
    screenshot: 'only-on-failure',
    video: process.env.CI ? 'retain-on-failure' : 'off'
  },
  outputDir: 'test-artifacts',
  projects
});



