using GCFoundation.Common.Utilities;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// A base class for form components that provides functionality for binding model properties, 
    /// performing validation, and adding common attributes like labels and hints to the HTML output.
    /// </summary>
    public abstract class FDCPBaseFormComponentTagHelper : BaseTagHelper
    {
        /// <summary>
        /// Options for serializing JSON property names in camel case.
        /// </summary>
        protected static readonly JsonSerializerOptions CamelCaseOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Retrieves the <see cref="DataTypeAttribute"/> for the property if available.
        /// </summary>
        protected DataTypeAttribute? DataTypeAttribute
        {
            get
            {
                if (PropertyInfo == null)
                {
                    return null;
                }
                return PropertyInfo.GetCustomAttribute<DataTypeAttribute>();
            }
        }

        /// <summary>
        /// Binds the tag helper to a model property, enabling validation and data binding.
        /// </summary>
        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; } = default!;

        /// <summary>
        /// Defines the <strong>id</strong> attribute of the form component.
        /// </summary>
        /// <remarks><em>Overrides the value derived from any bindings.</em></remarks>
        [HtmlAttributeName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Defines whether the form component is required or not.
        /// </summary>
        /// <remarks><em>Overrides the [Required] data annotation (if applicable).</em></remarks>
        [HtmlAttributeName("required")]
        public bool? IsRequired { get; set; }

        /// <summary>
        /// Defines the <strong>name</strong> attribute of the form component.
        /// </summary>
        /// <remarks><em>Overrides the value derived from any bindings.</em></remarks>
        [HtmlAttributeName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Retrieves the <see cref="PropertyInfo"/> for the model property bound to this tag helper.
        /// </summary>
        protected PropertyInfo? PropertyInfo
        {
            get
            {
                if (For == null || string.IsNullOrEmpty(For.Metadata.PropertyName))
                {
                    return null;
                }

                return For.Metadata.ContainerType?.GetProperty(For.Metadata.PropertyName);
            }
        }

        /// <summary>
        /// Injects the current ViewContext to access ModelState for validation.
        /// </summary>
        [ViewContext]
        public ViewContext ViewContext { get; set; } = default!;


        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            FormFieldContext field = ResolveFormField();

            output.TagName = "gcds-input";
            output.TagMode = TagMode.StartTagAndEndTag;

            AddAttributeIfNotNull(output, "value", field.Value ?? string.Empty);
            AddAttributeIfNotNull(output, "name", field.Name);
            AddAttributeIfNotNull(output, "label", field.Label);
            AddAttributeIfNotNull(output, "input-id", field.Id);
            AddAttributeIfNotNull(output, "hint", field.Hint);
            AddAttributeIfNotNull(output, "lang", LanguageUtility.GetCurrentApplicationLanguage());

            AddBooleanAttribute(output, "required", field.Required);
            AddAttributeIfNotNull(output, "validate-on", "blur");

            string? errorMessage = ResolveModelStateError(field.Name);
            AddAttributeIfNotNull(output, "error-message", errorMessage);
        }


        #region Helper classes
        /// <summary>
        /// Resolved field metadata shared by FDCP form component tag helpers.
        /// </summary>
        /// <param name="Name">The form field name.</param>
        /// <param name="Id">The form field id.</param>
        /// <param name="Label">The localized label or legend text.</param>
        /// <param name="Hint">The localized hint text.</param>
        /// <param name="Required">Whether the field is required.</param>
        /// <param name="Value">The string representation of the current value, if any.</param>
        /// <param name="Property">The bound model property, when resolved from <c>for</c>.</param>
        /// <param name="Model">The bound model value, when resolved from <c>for</c>.</param>
        protected sealed record FormFieldContext(
            string Name,
            string Id,
            string Label,
            string Hint,
            bool Required,
            string? Value,
            PropertyInfo? Property,
            object? Model);

        /// <summary>
        /// Optional overrides used when resolving form field metadata.
        /// </summary>
        protected sealed class FormFieldResolveOptions
        {
            /// <summary>
            /// Overrides the hint derived from model metadata.
            /// </summary>
            public string? Hint { get; init; }

            /// <summary>
            /// Overrides the label or legend derived from model metadata.
            /// </summary>
            public string? Label { get; init; }

            /// <summary>
            /// Overrides the value derived from model binding.
            /// </summary>
            public string? Value { get; init; }
        }
        #endregion
        #region Resolve methods
        /// <summary>
        /// Resolves common form field metadata from model binding and optional manual overrides.
        /// </summary>
        /// <param name="options">Optional manual overrides for label, hint, and value.</param>
        /// <returns>The resolved field metadata.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when metadata cannot be resolved from <c>for</c> or manual <c>name</c>.
        /// </exception>
        protected FormFieldContext ResolveFormField(FormFieldResolveOptions? options = null)
        {
            if (TryResolveFormField(out FormFieldContext field, options))
            {
                return field;
            }

            string message = (For != null ? "Missing properties" : "Either 'for' or 'name' must be specified.");
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Retrieves the localized label for a property, falling back to the property name if no label is provided.
        /// </summary>
        /// <param name="property">The property to retrieve the label for.</param>
        /// <returns>The localized label for the property.</returns>
        protected static string ResolveLocalizedLabel(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.GetName() ?? property.Name;
        }

        /// <summary>
        /// Retrieves the localized hint for a property, falling back to an empty string if no hint is provided.
        /// </summary>
        /// <param name="property">The property to retrieve the hint for.</param>
        /// <returns>The localized hint for the property.</returns>
        protected static string ResolveLocalizedHint(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.GetDescription() ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the first ModelState error message for a field, optionally only after form submission.
        /// </summary>
        protected string? ResolveModelStateError(string fieldName, bool onlyAfterSubmit = true)
        {
            if (onlyAfterSubmit)
            {
                bool formWasSubmitted = ViewContext?.HttpContext?.Request?.Method == "POST" ||
                                        ViewContext?.ModelState?.ErrorCount > 0;

                if (!formWasSubmitted)
                {
                    return null;
                }
            }

            if (ViewContext?.ModelState?.ContainsKey(fieldName) == true &&
                ViewContext.ModelState[fieldName]?.Errors?.Count > 0)
            {
                return ViewContext.ModelState[fieldName]!.Errors[0].ErrorMessage;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the bound field is required from metadata and tag helper overrides.
        /// </summary>
        protected bool ResolveRequired(PropertyInfo? property)
        {
            bool required = false;

            if (For != null)
                required = For.Metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any()
                           || property?.GetCustomAttribute<RequiredAttribute>() != null;

            if (IsRequired.HasValue)
                required = IsRequired.Value;

            return required;
        }

        /// <summary>
        /// Attempts to resolve common form field metadata from model binding and optional manual overrides.
        /// </summary>
        /// <param name="field">The resolved field metadata when successful.</param>
        /// <param name="options">Optional manual overrides for label, hint, and value.</param>
        /// <returns><c>true</c> when metadata was resolved; otherwise <c>false</c>.</returns>
        private bool TryResolveFormField(out FormFieldContext field, FormFieldResolveOptions? options = null)
        {
            options ??= new FormFieldResolveOptions();

            // If bound to a model property, resolve metadata from the expression and any overrides
            if (For != null)
            {
                PropertyInfo? property = PropertyInfo;
                string fieldName = Name ?? For.Name;
                string fieldId = Id ?? fieldName;

                if (property != null)
                {
                    field = new FormFieldContext(
                        fieldName,
                        fieldId,
                        options.Label ?? ResolveLocalizedLabel(property),
                        options.Hint ?? ResolveLocalizedHint(property),
                        ResolveRequired(property),
                        options.Value ?? For.Model?.ToString(),
                        property,
                        For.Model);
                    return true;
                }

                field = new FormFieldContext(
                    fieldName,
                    fieldId,
                    options.Label ?? For.Metadata.DisplayName ?? For.Name,
                    options.Hint ?? string.Empty,
                    ResolveRequired(null),
                    options.Value ?? For.Model?.ToString(),
                    null,
                    For.Model);
                return true;
            }

            // If not bound to a model property, attempt to resolve metadata from manual attributes
            if (string.IsNullOrWhiteSpace(Name))
            {
                field = default!;
                return false;
            }

            field = new FormFieldContext(
                Name,
                Id ?? Name,
                options.Label ?? string.Empty,
                options.Hint ?? string.Empty,
                IsRequired ?? false,
                options.Value,
                null,
                null);
            return true;
        }
        #endregion
    }
}