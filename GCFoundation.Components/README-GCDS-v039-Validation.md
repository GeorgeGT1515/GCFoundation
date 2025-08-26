# GCDS Components v0.39.0+ Validation Guide

## Overview

GCDS Components version 0.39.0 introduced breaking changes to form validation. The automatic error summary population was removed, and now only HTML5 native validation works by default. This guide shows how to restore the complete error summary functionality.

## What Changed

### Before v0.39.0 ❌
- `gcds-error-summary` automatically populated with all form validation errors
- Centralized error handling on form submission
- Error summary appeared automatically when validation failed

### After v0.39.0 ✅
- Only HTML5 native validation works (first error only)
- No automatic error summary population
- Manual error collection and summary population required

## Solution: Custom Validation Handler

We've implemented a custom JavaScript validation handler that restores the error summary functionality.

### Features
- ✅ Automatic error summary population
- ✅ Real-time field validation
- ✅ Server-side and client-side validation support
- ✅ Proper GCDS component integration
- ✅ Accessibility compliant

## Usage

### 1. Using FDCPFormTagHelper

```csharp
// In your Razor view
<fdcp-form for="Model" method="post" action="/submit">
    <gcds-input input-id="email" name="email" type="email" label="Email" required />
    <gcds-input input-id="phone" name="phone" type="tel" label="Phone" required />
    <gcds-button type="submit">Submit</gcds-button>
</fdcp-form>
```

**Generates:**
```html
<script src="~/js/gcds-validation-handler.js" defer></script>
<form method="post" action="/submit" data-gcds-validation="true" novalidate="true">
    <gcds-error-summary lang="en" style="display: none;"></gcds-error-summary>
    <!-- Your form fields -->
</form>
```

### 2. Using FDCPFormBuilderTagHelper

```csharp
// In your controller
var form = new FormDefinition
{
    Id = "contactForm",
    Title = "Contact Form",
    Action = "/contact/submit",
    Methode = "post",
    SubmithButtonText = "Send Message",
    Sections = new[]
    {
        new FormSection
        {
            Title = "Contact Information",
            Questions = new[]
            {
                new FormQuestion
                {
                    Id = "email",
                    Label = "Email Address",
                    Type = QuestionType.Email,
                    IsRequired = true,
                    ErrorMessage = "Please enter a valid email address"
                }
            }
        }
    }
};

// In your Razor view
<fdcp-form-builder form="@form" />
```

## Validation Behavior

### Client-Side Validation
1. **Form Submission**: Custom handler validates all fields
2. **Error Collection**: Collects all validation errors
3. **Error Summary**: Populates `gcds-error-summary` with error links
4. **Focus Management**: Focuses on first error field

### Server-Side Validation
1. **Model Errors**: Server-side errors automatically populate error summary
2. **Visibility**: Error summary is visible when server errors exist
3. **Persistence**: Client-side validation respects server-side errors

### Real-Time Validation
- **On Blur**: Fields validate when losing focus
- **On Input**: Errors clear as user types
- **Progressive Enhancement**: Works without JavaScript

## Validation Rules Supported

| Validation Type | GCDS Attribute | Custom Handler |
|----------------|----------------|----------------|
| Required | `required` | ✅ |
| Email | `type="email"` | ✅ |
| URL | `type="url"` | ✅ |
| Min Length | `minlength="n"` | ✅ |
| Max Length | `maxlength="n"` | ✅ |
| Pattern | `pattern="regex"` | ✅ |
| Custom Rules | `data-validation-rules` | ✅ |

## Error Summary Format

The error summary uses this JSON format for the `error-links` attribute:

```json
{
  "#fieldId1": "Error message 1",
  "#fieldId2": "Error message 2"
}
```

**Important**: Field IDs must be prefixed with `#` for proper linking.

## Testing

Your tests now verify:

### Form Attributes
```csharp
// Verify GCDS v0.39.0+ validation attributes
Assert.Equal("true", output.Attributes["data-gcds-validation"].Value);
Assert.Equal("true", output.Attributes["novalidate"].Value);
```

### Error Summary Behavior
```csharp
// With errors - visible
Assert.Contains("style=\"display: block;\"", content);
Assert.Contains("error-links", content);

// Without errors - hidden
Assert.Contains("style=\"display: none;\"", content);
Assert.DoesNotContain("error-links", content);
```

### Script Inclusion
```csharp
// Verify validation script is included
Assert.Contains("gcds-validation-handler.js", content);
```

## Migration Checklist

- [x] ✅ Include `gcds-validation-handler.js` in your application
- [x] ✅ Update tag helpers to use `data-gcds-validation="true"`
- [x] ✅ Add `novalidate="true"` to disable HTML5 validation
- [x] ✅ Update tests to verify new validation attributes
- [x] ✅ Test both client-side and server-side validation scenarios

## Troubleshooting

### Error Summary Not Appearing
1. Check that `gcds-validation-handler.js` is loaded
2. Verify form has `data-gcds-validation="true"` attribute
3. Ensure GCDS input fields have proper `input-id` attributes

### Validation Not Working
1. Check browser console for JavaScript errors
2. Verify GCDS components are properly initialized
3. Ensure field names match error summary field IDs

### Tests Failing
1. Update tests to check for new validation attributes
2. Verify error summary visibility logic
3. Check for validation script inclusion

## Browser Support

The validation handler supports:
- ✅ Modern browsers (Chrome 60+, Firefox 55+, Safari 11+)
- ✅ Progressive enhancement (graceful degradation)
- ✅ Screen readers and accessibility tools
- ✅ Mobile browsers

## Performance Considerations

- Script is loaded with `defer` attribute
- Validation runs only on form submission and field blur
- Error summary updates are minimal DOM manipulations
- Memory efficient with automatic cleanup
