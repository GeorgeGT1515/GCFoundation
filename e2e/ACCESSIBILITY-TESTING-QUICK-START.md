# Accessibility Testing - Quick Start Guide

Quick reference for running automated accessibility tests on the FDCP Grid Table component.

## 🚀 Quick Start

### 1. Start the Web Application
```bash
cd GCFoundation.Web
dotnet run
```
> Application will be available at https://localhost:7160 (or http://localhost:5215)

### 2. Run Accessibility Tests
In a new terminal:
```bash
cd e2e
npm run test:a11y:grid
```

## 📋 Common Commands

### Run All Grid Table Accessibility Tests
```bash
npm run test:a11y:grid
```
**Use when**: Testing all accessibility features of the Grid Table

### Run with Browser Visible (Headed Mode)
```bash
npm run test:a11y:grid:headed
```
**Use when**: Debugging test failures, want to see browser actions

### Run All Accessibility Tests (All Components)
```bash
npm run test:a11y
```
**Use when**: Testing entire application for accessibility

### Run Specific Test
```bash
npx playwright test fdcp-grid-table-a11y.spec.ts --grep "aria-sort"
```
**Use when**: Testing specific functionality

### View HTML Report
```bash
npx playwright show-report
```
**Use when**: Reviewing test results after a test run

### Run Tests on Specific Browser
```bash
npx playwright test fdcp-grid-table-a11y.spec.ts --project=chrome
npx playwright test fdcp-grid-table-a11y.spec.ts --project=edge
```
**Use when**: Testing browser-specific behavior

## 🔍 What Gets Tested

- ✅ WCAG 2.1 AAA compliance (axe-core automated checks)
- ✅ Table structure and semantic HTML
- ✅ `scope="col"` and `scope="row"` attributes
- ✅ `aria-sort` updates when sorting
- ✅ Keyboard navigation (Enter, Space, Tab)
- ✅ Focus indicators and management
- ✅ Screen reader support (ARIA labels, roles)
- ✅ Caption with `visibility-sr-only`
- ✅ Pagination accessibility
- ✅ Color contrast
- ✅ English and French versions

## 📊 Understanding Results

### ✅ All Tests Pass
```
Running 17 tests using 2 workers
  17 passed (30.0s)
```
**Action**: No issues found! ✅

### ❌ Test Failures
```
Running 17 tests using 2 workers
  15 passed
  2 failed
```
**Action**: 
1. Check test output for details
2. View HTML report: `npx playwright show-report`
3. Review screenshots in `test-artifacts/`
4. Fix issues and rerun

### Axe Violations
```
expect.soft(serious).toHaveLength(0)
Expected: 0
Received: 2
```
**Action**:
1. Open `axe-results.json` in test artifacts
2. Review violation details and affected elements
3. View `axe-overview.png` showing highlighted issues
4. Fix violations based on guidance
5. Rerun tests

## 🛠️ Troubleshooting

### "Failed to connect to https://localhost:7160"
**Problem**: Web application not running
**Solution**: Start `GCFoundation.Web` with `dotnet run`

### "Target closed" or "Navigation timeout"
**Problem**: Application is slow to start or crashed
**Solution**: 
- Restart the web application
- Increase timeout in `playwright.config.ts`

### Tests pass locally but fail in CI
**Problem**: Timing or environment differences
**Solution**:
- Check CI logs for specific errors
- Ensure CI has axe-core dependencies
- Verify baseURL configuration

### "Cannot find module" errors
**Problem**: Dependencies not installed
**Solution**:
```bash
cd e2e
npm install
```

## 📁 Output Files

After running tests, find results in:

```
e2e/
├── playwright-report/          # HTML report
│   └── index.html             # Open in browser
├── test-artifacts/            # Test output
│   ├── axe-results.json      # Full axe violations
│   ├── axe-summary.md        # Readable summary
│   ├── axe-report.html       # Visual report
│   ├── axe-overview.png      # Screenshot with highlights
│   └── *.png                 # Individual violation screenshots
└── test-results/
    └── junit.xml             # CI integration
```

## 🎯 Test Patterns

### Development Workflow
```bash
# 1. Make changes to Grid Table
# 2. Run accessibility tests
npm run test:a11y:grid

# 3. If failures, debug with headed mode
npm run test:a11y:grid:headed

# 4. View detailed report
npx playwright show-report

# 5. Fix issues and rerun
npm run test:a11y:grid
```

### Pre-Commit
```bash
# Quick accessibility check before committing
npm run test:a11y:grid
```

### CI/CD Pipeline
```bash
# Full test suite with artifacts
npm run test:a11y:ci
```

## 📚 Documentation

- **Full Documentation**: [tests/README-FDCP-GRID-TABLE-A11Y.md](./tests/README-FDCP-GRID-TABLE-A11Y.md)
- **Implementation Summary**: [tests/FDCP-GRID-TABLE-A11Y-SUMMARY.md](./tests/FDCP-GRID-TABLE-A11Y-SUMMARY.md)
- **Playwright Docs**: https://playwright.dev/docs/intro
- **axe-core Rules**: https://github.com/dequelabs/axe-core/blob/develop/doc/rule-descriptions.md

## 🔗 Quick Links

- [WCAG 2.1 Quick Reference](https://www.w3.org/WAI/WCAG21/quickref/)
- [GC Design System Accessibility](https://design-system.alpha.canada.ca/en/accessibility/)
- [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)

## ✨ Tips

- Run tests frequently during development
- Review actual violation details, not just pass/fail
- Test both English and French versions
- Use headed mode for debugging
- Keep axe-core updated for latest rules
- Add new tests when adding features

---

**Need Help?**
- Check [README-FDCP-GRID-TABLE-A11Y.md](./tests/README-FDCP-GRID-TABLE-A11Y.md) for detailed documentation
- Review test code in `tests/fdcp-grid-table-a11y.spec.ts`
- Check utility functions in `utils/grid-table-a11y.ts`

