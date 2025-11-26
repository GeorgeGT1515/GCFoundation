(() => {
    'use strict';

    const EDITOR_SELECTOR = '[data-fdcp-rich-text="true"]';
    const TEMPLATE_LABEL_DEFAULT = 'Insert template';

    function initWhenReady() {
        if (typeof window.Quill === 'undefined') {
            if (document.querySelector(EDITOR_SELECTOR)) {
                setTimeout(initWhenReady, 100);
            }
            return;
        }

        initRichText();
    }

    function initRichText() {
        const editors = document.querySelectorAll(EDITOR_SELECTOR);
        if (!editors.length) {
            return;
        }

        editors.forEach(setupEditorInstance);
    }

    function setupEditorInstance(editorContainer) {
        if (!editorContainer || editorContainer.dataset.quillInitialized === 'true') {
            return;
        }

        const inputId = editorContainer.getAttribute('data-for');
        if (!inputId) {
            return;
        }

        const hiddenInput = document.getElementById(inputId);
        if (!hiddenInput) {
            return;
        }

        const toolbarType = editorContainer.getAttribute('data-toolbar') || 'basic';
        const placeholder = editorContainer.getAttribute('data-placeholder') || '';
        const templatesPayload = editorContainer.getAttribute('data-templates');

        const modules = {
            toolbar: getToolbarConfig(toolbarType)
        };

        const quill = new window.Quill(editorContainer, {
            theme: 'snow',
            modules,
            placeholder
        });

        applyInitialValue(quill, hiddenInput);
        bindEditorEvents(quill, hiddenInput, editorContainer);
        enhanceAccessibility(editorContainer, hiddenInput);
        appendTemplateMenu(editorContainer, quill, templatesPayload, inputId);
        enhanceToolbarAccessibility(editorContainer, hiddenInput);
        enhanceTooltipAccessibility(editorContainer);

        editorContainer.dataset.quillInitialized = 'true';
    }

    function applyInitialValue(quill, hiddenInput) {
        if (!hiddenInput.value) {
            quill.setText('');
            return;
        }

        quill.clipboard.dangerouslyPasteHTML(hiddenInput.value);
    }

    function bindEditorEvents(quill, hiddenInput, editorContainer) {
        if (!quill || !hiddenInput) {
            return;
        }

        const wrapper = editorContainer.closest('.fdcp-rich-text-wrapper');

        quill.on('text-change', () => {
            const html = quill.root.innerHTML;
            const isEmpty = quill.getText().trim().length === 0;
            const newValue = isEmpty ? '' : html;

            if (hiddenInput.value !== newValue) {
                hiddenInput.value = newValue;
                triggerInputEvents(hiddenInput, newValue);
            }

            if (isEmpty) {
                removeErrorState(editorContainer, wrapper);
            }
        });

        hiddenInput.addEventListener('invalid', () => {
            addErrorState(editorContainer, wrapper);
        });

        hiddenInput.addEventListener('input', () => {
            removeErrorState(editorContainer, wrapper);
        });
    }

    function triggerInputEvents(hiddenInput, value) {
        hiddenInput.dispatchEvent(new Event('input', { bubbles: true }));
        hiddenInput.dispatchEvent(new Event('change', { bubbles: true }));
        hiddenInput.dispatchEvent(new CustomEvent('gcdsChange', { detail: value }));
    }

    function addErrorState(editorContainer, wrapper) {
        editorContainer.setAttribute('aria-invalid', 'true');
        if (wrapper) {
            wrapper.classList.add('has-error');
        }
    }

    function removeErrorState(editorContainer, wrapper) {
        editorContainer.removeAttribute('aria-invalid');
        if (wrapper) {
            wrapper.classList.remove('has-error');
        }
    }

    function enhanceAccessibility(editorContainer, hiddenInput) {
        const editorArea = editorContainer.querySelector('.ql-editor');
        if (!editorArea) {
            return;
        }

        const label = document.getElementById(`${hiddenInput.id}_label`) || document.querySelector(`label[for="${hiddenInput.id}"]`);
        if (label) {
            if (!label.id) {
                label.id = `${hiddenInput.id}-label`;
            }
            editorArea.setAttribute('aria-labelledby', label.id);
        }

        const describedBy = [];
        const hint = document.getElementById(`${hiddenInput.id}_hint`);
        if (hint) {
            describedBy.push(hint.id);
        }

        const error = document.getElementById(`${hiddenInput.id}_error`);
        if (error) {
            describedBy.push(error.id);
        }

        if (describedBy.length) {
            editorArea.setAttribute('aria-describedby', describedBy.join(' '));
        }

    }

    function appendTemplateMenu(editorContainer, quill, templatesPayload, inputId) {
        if (!templatesPayload) {
            return;
        }

        let templates;
        try {
            templates = JSON.parse(templatesPayload);
        } catch (error) {
            console.error('FDCP Rich Text: invalid template JSON', error);
            return;
        }

        if (!templates || typeof templates !== 'object') {
            return;
        }

        const entries = Object.entries(templates).filter(([_, value]) => typeof value === 'string' && value.trim().length);
        if (!entries.length) {
            return;
        }

        const wrapper = editorContainer.closest('.fdcp-rich-text-wrapper');
        if (!wrapper) {
            return;
        }

        const menu = document.createElement('div');
        menu.className = 'fdcp-rich-text-template-menu';

        const selectId = `${inputId}-template-select`;
        const label = document.createElement('label');
        label.className = 'fdcp-rich-text-template-label';
        label.id = `${inputId}-template-label`;
        label.textContent = TEMPLATE_LABEL_DEFAULT;
        label.setAttribute('for', selectId);

        const select = document.createElement('select');
        select.className = 'fdcp-rich-text-template-select';
        select.id = selectId;
        select.setAttribute('aria-labelledby', label.id);

        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = TEMPLATE_LABEL_DEFAULT;
        select.appendChild(defaultOption);

        entries.forEach(([name]) => {
            const option = document.createElement('option');
            option.value = name;
            option.textContent = name;
            select.appendChild(option);
        });

        select.addEventListener('change', (event) => {
            const key = event.target.value;
            if (!key || !templates[key]) {
                return;
            }

            const cursorIndex = quill.getSelection()?.index ?? quill.getLength();
            quill.clipboard.dangerouslyPasteHTML(cursorIndex, templates[key]);
            quill.focus();
            event.target.value = '';
        });

        menu.appendChild(label);
        menu.appendChild(select);

        const toolbar = wrapper.querySelector('.ql-toolbar');
        if (toolbar && toolbar.nextSibling) {
            wrapper.insertBefore(menu, toolbar.nextSibling);
        } else {
            wrapper.insertBefore(menu, editorContainer);
        }
    }

    function enhanceToolbarAccessibility(editorContainer, hiddenInput) {
        const wrapper = editorContainer.closest('.fdcp-rich-text-wrapper');
        const toolbar = wrapper?.querySelector('.ql-toolbar');
        if (!toolbar) {
            return;
        }

        const controlLabel = document.getElementById(`${hiddenInput.id}_label`);
        const labelText = controlLabel?.textContent?.trim();
        toolbar.setAttribute('role', 'toolbar');
        if (labelText) {
            toolbar.setAttribute('aria-label', `${labelText} formatting toolbar`);
        }

        toolbar.querySelectorAll('button').forEach(button => {
            const label = getButtonLabel(button);
            if (label && !button.getAttribute('aria-label')) {
                button.setAttribute('aria-label', label);
            }
        });

        toolbar.querySelectorAll('.ql-picker').forEach(picker => {
            const label = getPickerLabel(picker);
            if (!label) {
                return;
            }
            const trigger = picker.querySelector('.ql-picker-label');
            if (trigger && !trigger.getAttribute('aria-label')) {
                trigger.setAttribute('aria-label', label);
                trigger.setAttribute('role', 'button');
            }
        });

        toolbar.querySelectorAll('select').forEach(select => {
            const label = getPickerLabel(select);
            if (label && !select.getAttribute('aria-label')) {
                select.setAttribute('aria-label', label);
            }
        });
    }

    function enhanceTooltipAccessibility(editorContainer) {
        // The tooltip is often appended to the container, but sometimes to the body or elsewhere depending on config.
        // In standard snow theme, it is inside .ql-container, which is the editorContainer (since we init on it).
        // Or strictly speaking, Quill adds .ql-container class to the element we pass.
        // However, the tooltip might be created lazily. Usually it exists but is hidden.
        
        const wrapper = editorContainer.closest('.fdcp-rich-text-wrapper');
        if (!wrapper) return;

        // Locate the tooltip
        const tooltip = wrapper.querySelector('.ql-tooltip');
        if (!tooltip) return;

        // 1. Fix empty link preview
        const preview = tooltip.querySelector('a.ql-preview');
        if (preview) {
            if (!preview.getAttribute('aria-label')) {
                preview.setAttribute('aria-label', 'Current link URL');
            }
            if (!preview.textContent.trim()) {
                // Ideally it shows the URL, but if empty, screen readers need something.
                // Quill updates textContent when a link is selected.
                // If it is strictly empty, we can give it a title or label.
                // But the issue 'Empty link' suggests the link text is empty.
                // The aria-label should suffice for "A link contains no text".
            }
        }

        // 2. Fix missing form label for the input
        const input = tooltip.querySelector('input[type="text"]');
        if (input && !input.getAttribute('aria-label')) {
            // This input is used for Link, Video, Formula.
            // We can set a generic label or try to be specific if we detect mode (harder).
            input.setAttribute('aria-label', 'Enter link URL');
        }

        // 3. Fix ql-action (Save) and ql-remove (Remove)
        const actionBtn = tooltip.querySelector('a.ql-action');
        if (actionBtn) {
            if (!actionBtn.getAttribute('aria-label')) {
                actionBtn.setAttribute('aria-label', 'Save');
            }
            if (!actionBtn.getAttribute('role')) {
                actionBtn.setAttribute('role', 'button');
            }
        }

        const removeBtn = tooltip.querySelector('a.ql-remove');
        if (removeBtn) {
            if (!removeBtn.getAttribute('aria-label')) {
                removeBtn.setAttribute('aria-label', 'Remove');
            }
            if (!removeBtn.getAttribute('role')) {
                removeBtn.setAttribute('role', 'button');
            }
        }
    }

    function getButtonLabel(button) {
        const classList = Array.from(button.classList);
        if (classList.includes('ql-list')) {
            const value = button.getAttribute('value');
            return value === 'ordered' ? 'Numbered list' : 'Bulleted list';
        }
        if (classList.includes('ql-indent')) {
            const value = button.getAttribute('value');
            return value === '+1' ? 'Increase indent' : 'Decrease indent';
        }
        if (classList.includes('ql-script')) {
            const value = button.getAttribute('value');
            return value === 'sub' ? 'Subscript' : 'Superscript';
        }

        const simpleMap = {
            'ql-bold': 'Bold',
            'ql-italic': 'Italic',
            'ql-underline': 'Underline',
            'ql-strike': 'Strikethrough',
            'ql-link': 'Insert link',
            'ql-image': 'Insert image',
            'ql-video': 'Insert video',
            'ql-clean': 'Clear formatting'
        };

        const key = classList.find(cls => simpleMap[cls]);
        return key ? simpleMap[key] : null;
    }

    function getPickerLabel(picker) {
        const classList = Array.from(picker.classList);
        const map = {
            'ql-header': 'Formatting style',
            'ql-size': 'Font size',
            'ql-font': 'Font family',
            'ql-align': 'Text alignment',
            'ql-color': 'Text color',
            'ql-background': 'Background color'
        };
        const key = classList.find(cls => map[cls]);
        return key ? map[key] : null;
    }

    function getToolbarConfig(type) {
        switch (type) {
            case 'full':
                return [
                    [{ header: [2, 3, 4, 5, 6, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    [{ script: 'sub' }, { script: 'super' }],
                    [{ indent: '-1' }, { indent: '+1' }],
                    [{ align: [] }],
                    ['link', 'image', 'video'],
                    ['clean']
                ];
            case 'standard':
                return [
                    [{ header: [2, 3, 4, false] }],
                    ['bold', 'italic', 'underline', 'link'],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    ['clean']
                ];
            case 'basic':
            default:
                return [
                    ['bold', 'italic', 'underline'],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    ['link', 'clean']
                ];
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initWhenReady);
    } else {
        initWhenReady();
    }

    window.FDCP = window.FDCP || {};
    window.FDCP.initRichText = initRichText;
})();


