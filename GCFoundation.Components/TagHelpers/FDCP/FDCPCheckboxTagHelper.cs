using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Tag helper for rendering a single checkbox using the gcds-checkboxes component.
    /// </summary>
    [HtmlTargetElement("fdcp-checkbox")]
    public class FDCPCheckboxTagHelper : FDCPBaseFormComponentTagHelper
    {
        /// <summary>
        /// Gets or sets whether the checkbox is required.
        /// </summary>
        /// <remarks>
        /// This attribute will overwrite the [Required] data annotation (if applicable).
        /// </remarks>
        [HtmlAttributeName("required")]
        public bool? IsRequired { get; set; }

        private sealed class CheckboxOption
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("label")]
            public string Label { get; set; } = string.Empty;

            [JsonPropertyName("value")]
            public string Value { get; set; } = string.Empty;

            [JsonPropertyName("checked")]
            public bool Checked { get; set; }

            [JsonPropertyName("hint")]
            public string? Hint { get; set; }
        }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (For == null)
            {
                throw new InvalidOperationException("For is NULL in FDCPCheckbox.");
            }

            string fieldName = For.Name;
            PropertyInfo? propertyInfo = PropertyInfo;

            if (propertyInfo == null)
            {
                throw new InvalidOperationException("Missing properties");
            }

            string label = GetLocalizedLabel(propertyInfo);
            string hint = GetLocalizedHint(propertyInfo);
            bool required = For.Metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any()
                            || propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;

            // Get the current value
            var currentValue = For.Model as bool? ?? false;

            output.TagName = "gcds-checkboxes";
            output.TagMode = TagMode.StartTagAndEndTag;

            // Create the single checkbox option
            var option = new CheckboxOption
            {
                Id = fieldName,
                Label = label,
                Value = "true",
                @Checked = currentValue,
                Hint = hint
            };

            // Set the required attributes
            AddAttributeIfNotNull(output, "legend", label);
            AddAttributeIfNotNull(output, "name", fieldName);
            AddAttributeIfNotNull(output, "options", JsonSerializer.Serialize(new[] { option }, CamelCaseOptions));

            // If a "required" attribute was defined on the tag helper, use that value.
            // Otherwise, look at the default [Required] data annotation.
            if (IsRequired.HasValue) required = IsRequired.Value;
            if (required)
            {
                output.Attributes.SetAttribute("required", "");
            }

            // Clear the content since we're using options attribute
            output.Content.SetHtmlContent(string.Empty);
        }
    }
}
