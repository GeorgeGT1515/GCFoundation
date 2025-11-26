# Run accessibility tests

## Overview
Execute the full accessibility (a11y) tests and systematically fix any failures, ensuring code quality and functionality.

## Setps

1. **Runt the application**
    - Start the application if not running `dotnet run --project GCFoundation.Web/GCFoundation.Web.csproj --urls http://localhost:5215` if running kill the process and restart the application
    - Capture the output and identify when the application run

2.  **Run accessibility tests**
    - Make sure to be in the appropriate folder e2e
    - Run the test `npm run test:a11y`
    - Capture the output and identify when the tests are done
3. **Analyze failures**
    - Categorize by type: flaky, broken, new failures
    - Prioritize fixes based on impact
    - Check if failures are related to recent changes
3. **Fix issues systematically**
    - Start with the most critical failures
    - Fix one issue at a time
    - Re-run tests after each fix

4. **Clean-up**
    - Make sure the application is shutdown after the process