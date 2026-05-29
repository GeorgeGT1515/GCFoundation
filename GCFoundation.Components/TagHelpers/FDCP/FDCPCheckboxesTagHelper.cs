using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// A tag helper for rendering a group of checkboxes using the gcds-checkboxes component.
    /// It binds to a model property and renders checkboxes based on the provided items.
    /// </summary>
    [HtmlTargetElement("fdcp-checkboxes", Attributes = "for, items")]
    [HtmlTargetElement("fdcp-checkboxes", Attributes = "items, name")]
    public class FDCPCheckboxesTagHelper : FDCPBaseFormComponentTagHelper
    {
        /// <summary>
        /// The list of items to be rendered as checkboxes.
        /// Each item should have a text (label) and value (for the checkbox).
        /// </summary>
        [HtmlAttributeName("items")]
        public IEnumerable<SelectListItem> Items { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Legend text for the checkbox group.
        /// </summary>
        [HtmlAttributeName("legend")]
        public string? Legend { get; set; }

        /// <summary>
        /// Hint text for the checkbox group.
        /// </summary>
        [HtmlAttributeName("hint")]
        public string? Hint { get; set; }

        /// <summary>
        /// Comma-separated selected values when <c>for</c> is not specified,
        /// or overrides the bound model value when <c>for</c> is specified.
        /// </summary>
        [HtmlAttributeName("value")]
        public string? Value { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            FormFieldContext field = ResolveFormField(new FormFieldResolveOptions
            {
                Label = Legend,
                Hint = Hint,
                Value = Value,
                MissingBindingMessage = For != null ? "Missing properties" : "Either 'for' or 'name' must be specified."
            });

            var selectedValues = GetSelectedValues(field);

            output.TagName = "gcds-checkboxes";
            output.TagMode = TagMode.StartTagAndEndTag;

            var options = Items.Select(item => new
            {
                id = $"{field.Id}_{item.Value}",
                label = item.Text,
                value = item.Value,
                @checked = selectedValues.Contains(item.Value),
            });

            AddAttributeIfNotNull(output, "name", field.Name);
            AddAttributeIfNotNull(output, "legend", field.Label);
            AddAttributeIfNotNull(output, "hint", field.Hint);
            AddAttributeIfNotNull(output, "options", JsonSerializer.Serialize(options));

            ApplyGcdsRequiredAttribute(output, field.Required, useEmptyStringValue: true);

            output.Content.SetHtmlContent(string.Empty);
        }

        private static List<string> GetSelectedValues(FormFieldContext field)
        {
            if (field.Model is List<string> list)
            {
                return list;
            }

            if (field.Model is IEnumerable<string> values)
            {
                return values.ToList();
            }

            if (string.IsNullOrWhiteSpace(field.Value))
            {
                return new List<string>();
            }

            return field.Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
    }
}
