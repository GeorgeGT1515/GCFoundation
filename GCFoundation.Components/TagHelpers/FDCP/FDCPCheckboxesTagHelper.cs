using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// A tag helper for rendering a group of checkboxes using the gcds-checkboxes component.
    /// It binds to a model property and renders checkboxes based on the provided items.
    /// </summary>
    [HtmlTargetElement("fdcp-checkboxes", Attributes = "for, items")]
    public class FDCPCheckboxesTagHelper : FDCPBaseFormComponentTagHelper
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
        public bool? IsRequired { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (For == null)
            {
                throw new InvalidOperationException("For is NULL in FDCPCheckboxes.");
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

            // Retrieve selected values (if any)
            var selectedValues = For.Model as List<string> ?? new List<string>();

            output.TagName = "gcds-checkboxes";
            output.TagMode = TagMode.StartTagAndEndTag;

            // Convert SelectListItems to the required options format
            var options = Items.Select(item => new
            {
                id = $"{fieldId}_{item.Value}",
                label = item.Text,
                value = item.Value,
                @checked = selectedValues.Contains(item.Value),
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
