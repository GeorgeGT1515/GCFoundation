using GCFoundation.Common.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Text;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders a custom dropdown (select) component.
    /// Use &lt;fdcp-select&gt; in your Razor views to generate a dropdown list.
    /// </summary>
    [HtmlTargetElement("fdcp-select", Attributes = "for, items")]
    [HtmlTargetElement("fdcp-select", Attributes = "items, name")]
    public class FDCPSelectTagHelper : FDCPBaseFormComponentTagHelper
    {
        /// <summary>
        /// Gets or sets the default selected value in the dropdown.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Label text for the select. Used when <c>for</c> is not specified,
        /// or overrides the model display name when <c>for</c> is specified.
        /// </summary>
        [HtmlAttributeName("label")]
        public string? Label { get; set; }

        /// <summary>
        /// Hint text for the select.
        /// </summary>
        [HtmlAttributeName("hint")]
        public string? Hint { get; set; }

        /// <summary>
        /// The currently selected value.
        /// </summary>
        [HtmlAttributeName("value")]
        public string? Value { get; set; }

        /// <summary>
        /// The list of selectable options for the dropdown.
        /// </summary>
        [HtmlAttributeName("items")]
        public IEnumerable<SelectListItem> Items { get; set; } = new List<SelectListItem>();

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (!TryResolveFormField(out FormFieldContext field, new FormFieldResolveOptions
            {
                Label = Label,
                Hint = Hint,
                Value = Value
            }))
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "gcds-select";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("name", field.Name);
            output.Attributes.SetAttribute("select-id", field.Id);
            output.Attributes.SetAttribute("class", "gcds-select");
            output.Attributes.SetAttribute("label", field.Label);
            output.Attributes.SetAttribute("lang", LanguageUtility.GetCurrentApplicationLanguage());

            if (!string.IsNullOrEmpty(field.Hint))
            {
                output.Attributes.SetAttribute("hint", field.Hint);
            }

            if (!string.IsNullOrWhiteSpace(DefaultValue))
            {
                output.Attributes.SetAttribute("default-value", DefaultValue);
            }

            if (field.Required)
            {
                ApplyGcdsRequiredAttribute(output, field.Required);
                output.Attributes.SetAttribute("validate-on", "blur");

                string? errorMessage = ResolveModelStateError(field.Name);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    output.Attributes.SetAttribute("error-message", errorMessage);
                }
            }

            string? selectedValue = field.Value ?? field.Model?.ToString();
            var sb = new StringBuilder();

            foreach (var item in Items)
            {
                var selected = selectedValue == item.Value ? " selected" : "";
                sb.AppendLine(CultureInfo.InvariantCulture, $"<option value='{item.Value}'{selected}>{item.Text}</option>");
            }

            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
