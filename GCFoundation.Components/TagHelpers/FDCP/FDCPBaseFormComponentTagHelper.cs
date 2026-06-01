using GCFoundation.Common.Utilities;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// A base class for form components that provides functionality for binding model properties, 
    /// performing validation, and adding common attributes like labels and hints to the HTML output.
    /// </summary>
    public abstract class FDCPBaseFormComponentTagHelper : BaseFormFieldTagHelper
    {
        /// <summary>
        /// Options for serializing JSON property names in camel case.
        /// </summary>
        protected static readonly JsonSerializerOptions CamelCaseOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
    }
}