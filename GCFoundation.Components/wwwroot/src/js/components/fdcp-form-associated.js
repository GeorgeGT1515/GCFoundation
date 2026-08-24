class FDCPFormAssociated {
    constructor() {
        this._initialValues = new Map();
    }

    _captureInitialValue(input) {
        if (!input) return;

        if (input.type === 'checkbox' || input.type === 'radio') {
            this._initialValues.set(input, input.defaultChecked);
        } else {
            this._initialValues.set(input, input.defaultValue);
        }
    }

    _bindResetListener() {
        if (!this.form) return;

        this.form.addEventListener('reset', () => this.formResetCallback());
    }

    formResetCallback() {}
}
