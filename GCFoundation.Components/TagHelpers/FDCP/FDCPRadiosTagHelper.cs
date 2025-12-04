using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders a custom radio button component wrapped in a `gcds-fieldset` element.
    /// Use &lt;fdcp-radio&gt; in your Razor views to generate a radio button group.
    /// </summary>
    [HtmlTargetElement("fdcp-radios", Attributes = "for, items")]
    public class FDCPRadiosTagHelper : FDCPBaseFormComponentTagHelper
    {
        /// <summary>
        /// The list of items to be rendered as checkboxes.
        /// Each item should have a text (label) and value (for the checkbox).
        /// </summary>
        [HtmlAttributeName("items")]
        public IEnumerable<SelectListItem> Items { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Gets or sets whether the checkbox group is required.
        /// </summary>
        /// <remarks>
        /// This attribute will overwrite the [Required] data annotation (if applicable).
        /// </remarks>
        [HtmlAttributeName("required")]
        public new bool? IsRequired { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (For == null)
            {
                throw new InvalidOperationException("For is NULL in FDCPRadios.");
            }
            
            PropertyInfo? propertyInfo = PropertyInfo;
            if (propertyInfo == null)
            {
                throw new InvalidOperationException("Missing properties");
            }

            string fieldName = Name ?? For.Name;
            string fieldId = Id ?? fieldName;
            string legend = GetLocalizedLabel(propertyInfo);
            string hint = GetLocalizedHint(propertyInfo);
            bool required = For.Metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any()
                            || propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;

            // Retrieve the selected value (if any) & bind as string
            var selectedValue = For.Model?.ToString() ?? string.Empty;

            output.TagName = "gcds-radios";
            output.TagMode = TagMode.StartTagAndEndTag;

            // Convert SelectListItems to the required options format
            var options = Items.Select(item => new
            {
                id = $"{fieldId}_{item.Value}",
                label = item.Text,
                value = item.Value,
                @checked = selectedValue == item.Value
            });

            AddAttributeIfNotNull(output, "name", fieldName);
            AddAttributeIfNotNull(output, "legend", legend);
            AddAttributeIfNotNull(output, "hint", hint);
            AddAttributeIfNotNull(output, "options", JsonSerializer.Serialize(options));

            // If a "required" attribute was defined on the tag helper, use that value.
            // Otherwise, look at the default [Required] data annotation.
            if (IsRequired.HasValue) required = IsRequired.Value;
            if (required)
            {
                output.Attributes.SetAttribute("required", "");
            }

            // Clear the content since we're using the options attribute
            output.Content.SetHtmlContent(string.Empty);
        }
    }
}
