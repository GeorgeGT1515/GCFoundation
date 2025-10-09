# FDCP Grid Table - Automated Accessibility Testing

This document describes the automated accessibility tests for the FDCP Grid Table component.

## Overview

The FDCP Grid Table accessibility tests ensure WCAG 2.1 AAA compliance and verify that the component is fully accessible to users with disabilities, including those using screen readers, keyboard-only navigation, and assistive technologies.

## Test Coverage

### 1. WCAG 2.1 AAA Compliance (`test:a11y`)
- Runs axe-core automated accessibility checks with WCAG 2.1 AAA rules
- Detects serious and critical violations
- Generates detailed reports with violation summaries and screenshots

### 2. Table Structure & Semantics
- ✅ Proper semantic HTML (`<table>`, `<thead>`, `<tbody>`, `<tr>`, `<th>`, `<td>`)
- ✅ Table caption with screen-reader-only visibility (`visibility-sr-only`)
- ✅ `scope="col"` on all column headers
- ✅ `scope="row"` on first cell of each row
- ✅ No empty headers

### 3. ARIA Attributes
- ✅ `aria-sort` on sortable column headers (none/ascending/descending)
- ✅ `aria-sort` updates dynamically when columns are sorted
- ✅ `aria-label` or `aria-labelledby` on table container
- ✅ Proper ARIA roles on pagination elements

### 4. Keyboard Navigation
- ✅ All interactive elements are keyboard accessible (Tab/Shift+Tab)
- ✅ Column headers can be sorted with Enter and Space keys
- ✅ Pagination buttons respond to keyboard input
- ✅ Search input supports keyboard input
- ✅ Focus is maintained after sort operations

### 5. Focus Management
- ✅ Visible focus indicators on all interactive elements
- ✅ Logical tab order
- ✅ Focus remains on sorted header after sort

### 6. Screen Reader Support
- ✅ Pagination summary has `role="status"` for live announcements
- ✅ Table caption is announced by screen readers
- ✅ Sort state is announced via `aria-sort`
- ✅ All labels and descriptions are properly associated

### 7. Internationalization (i18n)
- ✅ Component is fully accessible in English
- ✅ Component is fully accessible in French
- ✅ ARIA attributes and labels are localized
- ✅ No accessibility regressions when switching languages

### 8. Color Contrast
- ✅ Text meets WCAG AAA contrast requirements (7:1 for normal text)
- ✅ Interactive elements have sufficient contrast
- ✅ Focus indicators are visible against all backgrounds

## Running Tests

### Prerequisites
1. Ensure the web application is running:
   ```bash
   cd ../GCFoundation.Web
   dotnet run
   ```

2. Navigate to the e2e directory:
   ```bash
   cd e2e
   ```

### Run All Accessibility Tests
```bash
npm run test:a11y
```

### Run FDCP Grid Table Accessibility Tests Only
```bash
npm run test:a11y:grid
```

### Run with Browser UI (Headed Mode)
```bash
npm run test:a11y:grid:headed
```

### Run Specific Test
```bash
npx playwright test fdcp-grid-table-a11y.spec.ts --grep "should have scope"
```

### Run in CI/CD
```bash
npm run test:a11y:ci
```

## Test Reports

### HTML Report
After running tests, view the HTML report:
```bash
npx playwright show-report
```

### Axe Results
Each test generates:
- `axe-results.json`: Full axe-core results
- `axe-summary.md`: Markdown summary of violations
- `axe-report.html`: HTML report (if axe-html-reporter is installed)
- `axe-overview.png`: Screenshot highlighting violations
- Individual screenshots for each serious/critical violation

### JUnit Report
For CI integration, JUnit XML is generated at:
```
e2e/test-results/junit.xml
```

## Understanding Results

### Violation Severity Levels
- **Critical**: Must fix - major accessibility barrier
- **Serious**: Should fix - significant accessibility issue
- **Moderate**: Should fix when possible
- **Minor**: Nice to fix

### Common Issues and Fixes

#### Missing `scope` Attributes
**Issue**: Column headers without `scope="col"`
**Fix**: Verify `updateAriaSort` function in `fdcp-grid-table.js` sets scope attributes

#### Incorrect `aria-sort` Values
**Issue**: `aria-sort` not updating or has invalid value
**Fix**: Check MutationObserver and Grid.js event listeners in `fdcp-grid-table.js`

#### Missing Focus Indicators
**Issue**: Interactive elements don't show visible focus
**Fix**: Add focus styles in `_fdcp-grid-table.scss`

#### Color Contrast Failures
**Issue**: Text doesn't meet WCAG AAA 7:1 contrast ratio
**Fix**: Update color variables in SCSS to use GCDS tokens with sufficient contrast

## Continuous Monitoring

### Local Development
Run accessibility tests before committing:
```bash
npm run test:a11y:grid
```

### Pre-commit Hook (Optional)
Add to `.git/hooks/pre-commit`:
```bash
#!/bin/bash
cd e2e
npm run test:a11y:grid || exit 1
```

### CI/CD Pipeline
The tests are integrated into the Azure Pipeline:
```yaml
- script: npm run test:a11y:ci
  workingDirectory: e2e
  displayName: 'Run Accessibility Tests'
```

## Utility Functions

The test suite includes reusable utility functions in `e2e/utils/grid-table-a11y.ts`:

### `checkGridTableAccessibility(page, selector)`
Performs comprehensive accessibility check and returns results object.

### `testSortAccessibility(page, headerIndex)`
Tests sorting functionality and aria-sort updates.

### `testKeyboardNavigation(page, element)`
Verifies element is keyboard accessible.

### `hasFocusIndicator(locator)`
Checks if element has visible focus indicator.

### `checkPaginationAccessibility(page)`
Verifies pagination has proper screen reader support.

### `verifyKeyboardAccessibility(page, selectors)`
Tests keyboard accessibility for multiple elements.

### `getScreenReaderText(locator)`
Retrieves all screen reader text attributes.

## Best Practices

1. **Run tests frequently**: Test after every accessibility-related change
2. **Review reports**: Don't just check pass/fail - review violation details
3. **Test with real assistive technologies**: Automated tests catch ~30-40% of issues
4. **Test both languages**: Ensure accessibility in English and French
5. **Update tests**: Add new tests when adding features
6. **Document fixes**: Note accessibility fixes in commit messages

## Manual Testing Checklist

While automated tests catch many issues, also perform manual testing:

- [ ] Test with NVDA/JAWS screen reader
- [ ] Navigate entire table with keyboard only
- [ ] Test with Windows High Contrast mode
- [ ] Test with browser zoom at 200%
- [ ] Test with keyboard shortcuts
- [ ] Test with voice control software

## Resources

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [GC Design System Accessibility](https://design-system.alpha.canada.ca/en/accessibility/)
- [axe-core Rules](https://github.com/dequelabs/axe-core/blob/develop/doc/rule-descriptions.md)
- [ARIA Authoring Practices - Tables](https://www.w3.org/WAI/ARIA/apg/patterns/table/)
- [Playwright Accessibility Testing](https://playwright.dev/docs/accessibility-testing)

## Support

For questions or issues with accessibility tests:
1. Check this documentation
2. Review test output and reports
3. Consult WCAG guidelines
4. Open an issue in the repository

