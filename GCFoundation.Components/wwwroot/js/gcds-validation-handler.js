/**
 * GCDS Components v0.39.0+ Validation Handler
 * Restores automatic error summary functionality that was removed in v0.39.0
 */

class GCDSValidationHandler {
    constructor(formSelector = 'form[data-gcds-validation="true"]') {
        this.forms = document.querySelectorAll(formSelector);
        this.init();
    }

    init() {
        this.forms.forEach(form => {
            this.setupFormValidation(form);
        });
    }

    setupFormValidation(form) {
        // Prevent default HTML5 validation to handle it manually
        form.setAttribute('novalidate', 'true');
        
        // Add submit event listener
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            this.validateForm(form);
        });

        // Add real-time validation for individual fields
        const inputs = form.querySelectorAll('gcds-input, gcds-textarea, gcds-select');
        inputs.forEach(input => {
            this.setupFieldValidation(input);
        });
    }

    setupFieldValidation(fieldElement) {
        // Get the actual input element within the GCDS component
        const inputElement = fieldElement.querySelector('input, textarea, select');
        if (!inputElement) return;

        // Validate on blur (when field loses focus)
        inputElement.addEventListener('blur', () => {
            this.validateField(fieldElement, inputElement);
        });

        // Clear errors on input
        inputElement.addEventListener('input', () => {
            this.clearFieldError(fieldElement);
        });
    }

    validateField(fieldElement, inputElement) {
        const errors = [];
        const fieldId = fieldElement.getAttribute('input-id') || fieldElement.getAttribute('textarea-id') || inputElement.id;
        
        // Required field validation
        if (inputElement.hasAttribute('required') && !inputElement.value.trim()) {
            const label = fieldElement.getAttribute('label') || 'This field';
            errors.push(`${label} is required`);
        }

        // Email validation
        if (inputElement.type === 'email' && inputElement.value) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(inputElement.value)) {
                errors.push('Please enter a valid email address');
            }
        }

        // URL validation
        if (inputElement.type === 'url' && inputElement.value) {
            try {
                new URL(inputElement.value);
            } catch {
                errors.push('Please enter a valid URL');
            }
        }

        // Length validation
        if (inputElement.hasAttribute('minlength')) {
            const minLength = parseInt(inputElement.getAttribute('minlength'));
            if (inputElement.value.length < minLength) {
                errors.push(`Must be at least ${minLength} characters long`);
            }
        }

        if (inputElement.hasAttribute('maxlength')) {
            const maxLength = parseInt(inputElement.getAttribute('maxlength'));
            if (inputElement.value.length > maxLength) {
                errors.push(`Must be no more than ${maxLength} characters long`);
            }
        }

        // Pattern validation
        if (inputElement.hasAttribute('pattern') && inputElement.value) {
            const pattern = new RegExp(inputElement.getAttribute('pattern'));
            if (!pattern.test(inputElement.value)) {
                const title = inputElement.getAttribute('title') || 'Invalid format';
                errors.push(title);
            }
        }

        // Update field error state
        if (errors.length > 0) {
            this.setFieldError(fieldElement, errors[0]);
            return false;
        } else {
            this.clearFieldError(fieldElement);
            return true;
        }
    }

    validateForm(form) {
        const errors = {};
        let isValid = true;

        // Validate all fields
        const inputs = form.querySelectorAll('gcds-input, gcds-textarea, gcds-select');
        inputs.forEach(fieldElement => {
            const inputElement = fieldElement.querySelector('input, textarea, select');
            if (inputElement) {
                const fieldValid = this.validateField(fieldElement, inputElement);
                if (!fieldValid) {
                    isValid = false;
                    const fieldId = fieldElement.getAttribute('input-id') || fieldElement.getAttribute('textarea-id') || inputElement.id;
                    const errorMessage = fieldElement.getAttribute('error-message') || 'This field has an error';
                    errors[`#${fieldId}`] = errorMessage;
                }
            }
        });

        // Update error summary
        this.updateErrorSummary(form, errors);

        // If valid, submit the form
        if (isValid) {
            this.submitForm(form);
        } else {
            // Focus on first error
            this.focusFirstError(form);
        }
    }

    setFieldError(fieldElement, errorMessage) {
        fieldElement.setAttribute('error-message', errorMessage);
        fieldElement.setAttribute('invalid', 'true');
    }

    clearFieldError(fieldElement) {
        fieldElement.removeAttribute('error-message');
        fieldElement.removeAttribute('invalid');
    }

    updateErrorSummary(form, errors) {
        let errorSummary = form.querySelector('gcds-error-summary');
        
        if (Object.keys(errors).length > 0) {
            // Create error summary if it doesn't exist
            if (!errorSummary) {
                errorSummary = document.createElement('gcds-error-summary');
                // Insert at the beginning of the form
                form.insertBefore(errorSummary, form.firstChild);
            }
            
            // Update error-links attribute
            errorSummary.setAttribute('error-links', JSON.stringify(errors));
            errorSummary.style.display = 'block';
        } else if (errorSummary) {
            // Hide error summary if no errors
            errorSummary.style.display = 'none';
            errorSummary.removeAttribute('error-links');
        }
    }

    focusFirstError(form) {
        const firstErrorField = form.querySelector('[invalid="true"] input, [invalid="true"] textarea, [invalid="true"] select');
        if (firstErrorField) {
            firstErrorField.focus();
        }
    }

    submitForm(form) {
        // Re-enable default submission or trigger custom submission logic
        const submitEvent = new Event('submit', { bubbles: true, cancelable: true });
        form.removeAttribute('novalidate');
        
        // Remove our custom handler temporarily to allow normal submission
        const originalHandler = form.onsubmit;
        form.onsubmit = null;
        
        form.dispatchEvent(submitEvent);
        
        // Restore handler
        form.onsubmit = originalHandler;
    }
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new GCDSValidationHandler();
});

// Export for manual initialization
window.GCDSValidationHandler = GCDSValidationHandler;