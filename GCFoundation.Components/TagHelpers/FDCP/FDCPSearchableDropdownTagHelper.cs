using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders a searchable dropdown that supports single and multiple selection.
    /// </summary>
    [HtmlTargetElement("fdcp-searchable-dropdown", Attributes = "for, items")]
    [HtmlTargetElement("fdcp-searchable-dropdown", Attributes = "items, name")]
    public class FDCPSearchableDropdownTagHelper : FDCPBaseFormComponentTagHelper
    {
        /// <summary>
        /// The text shown in the trigger when no option is selected.
        /// </summary>
        public string DefaultValue { get; set; } = "Select option";

        /// <summary>
        /// Label text for the dropdown. Used when <c>for</c> is not specified,
        /// or overrides the model display name when <c>for</c> is specified.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// The list of selectable options.
        /// </summary>
        [HtmlAttributeName("items")]
        public IEnumerable<SelectListItem> Items { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Determines whether the dropdown allows one or many selected options.
        /// </summary>
        public FDCPSearchableDropdownSelectionMode SelectionMode { get; set; } = FDCPSearchableDropdownSelectionMode.Single;

        /// <summary>
        /// Placeholder text for the search input.
        /// </summary>
        public string SearchPlaceholder { get; set; } = "Search";

        /// <summary>
        /// Accessible label for the search input.
        /// </summary>
        public string SearchLabel { get; set; } = "Search options";

        /// <summary>
        /// Text shown when no options match the search term.
        /// </summary>
        public string NoResultsText { get; set; } = "No results found";

        /// <summary>
        /// Text announced when one option matches the search term.
        /// </summary>
        public string OneResultText { get; set; } = "1 result available";

        /// <summary>
        /// Text announced when multiple options match the search term. Use <c>{0}</c> for the result count.
        /// </summary>
        public string MultipleResultsText { get; set; } = "{0} results available";

        /// <summary>
        /// Text shown after the selected count in multiple selection mode.
        /// </summary>
        public string MultipleSelectedText { get; set; } = "selected";

        /// <inheritdoc/>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            FormFieldContext field = ResolveFormField(new FormFieldResolveOptions
            {
                Label = Label,
                Hint = Hint,
                Value = Value
            });

            var selectedValues = GetSelectedValues(field);
            var items = Items.ToList();
            foreach (var selectedItem in items.Where(item => item.Selected))
            {
                selectedValues.Add(selectedItem.Value);
            }

            var selectedLabels = items
                .Where(item => selectedValues.Contains(item.Value))
                .Select(item => item.Text)
                .ToList();

            string componentId = SanitizeId(field.Id);
            string labelId = $"{componentId}_label";
            string triggerId = $"{componentId}_trigger";
            string panelId = $"{componentId}_panel";
            string searchId = $"{componentId}_search";
            string searchLabelId = $"{componentId}_search_label";
            string optionsId = $"{componentId}_options";
            string statusId = $"{componentId}_status";
            string hintId = $"{componentId}_hint";
            string errorId = $"{componentId}_error";
            string footerSlot = await GetSlotContentAsync(output).ConfigureAwait(true);
            string mode = SelectionMode.ToString().ToLowerInvariant();
            string selectedSummary = selectedLabels.Count == 0
                ? DefaultValue
                : SelectionMode == FDCPSearchableDropdownSelectionMode.Multiple
                    ? $"{selectedLabels.Count} {MultipleSelectedText}"
                    : string.Join(", ", selectedLabels);

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", $"fdcp-searchable-dropdown fdcp-searchable-dropdown--{mode}");
            output.Attributes.SetAttribute("id", componentId);
            output.Attributes.SetAttribute("data-fdcp-searchable-dropdown", string.Empty);
            output.Attributes.SetAttribute("data-selection-mode", mode);
            output.Attributes.SetAttribute("data-default-value", DefaultValue);
            output.Attributes.SetAttribute("data-multiple-selected-text", MultipleSelectedText);
            output.Attributes.SetAttribute("data-one-result-text", OneResultText);
            output.Attributes.SetAttribute("data-multiple-results-text", MultipleResultsText);

            var sb = new StringBuilder();
            sb.AppendLine(CultureInfo.InvariantCulture, $"<label class=\"fdcp-searchable-dropdown__label gcds-label\" id=\"{EncodeAttribute(labelId)}\" for=\"{EncodeAttribute(triggerId)}\">{Encode(field.Label)}</label>");

            if (!string.IsNullOrWhiteSpace(field.Hint))
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"<gcds-hint hint-id=\"{EncodeAttribute(hintId)}\" id=\"{EncodeAttribute(hintId)}\">{Encode(field.Hint)}</gcds-hint>");
            }

            string describedBy = BuildDescribedBy(field.Hint, hintId, ResolveModelStateError(field.Name), errorId);
            string ariaDescribedBy = string.IsNullOrWhiteSpace(describedBy) ? string.Empty : $" aria-describedby=\"{EncodeAttribute(describedBy)}\"";
            string disabled = field.Disabled ? " disabled" : string.Empty;
            string required = field.Required ? " aria-required=\"true\"" : string.Empty;
            string ariaHasPopup = SelectionMode == FDCPSearchableDropdownSelectionMode.Single ? @" aria-haspopup=""listbox""" : string.Empty;
            string searchComboboxAttributes = SelectionMode == FDCPSearchableDropdownSelectionMode.Single
                ? $@" role=""combobox"" aria-autocomplete=""list"" aria-expanded=""false"" aria-controls=""{EncodeAttribute(optionsId)}"" aria-labelledby=""{EncodeAttribute(labelId)} {EncodeAttribute(searchLabelId)}"" aria-describedby=""{EncodeAttribute(statusId)}"""
                : string.Empty;

            sb.AppendLine(CultureInfo.InvariantCulture, $@"<button type=""button""
    class=""fdcp-searchable-dropdown__trigger""
    id=""{EncodeAttribute(triggerId)}""
    aria-expanded=""false""
    aria-controls=""{EncodeAttribute(panelId)}""
    data-fdcp-searchable-dropdown-trigger{ariaHasPopup}{ariaDescribedBy}{required}{disabled}>
    <span class=""fdcp-searchable-dropdown__trigger-text"" data-fdcp-searchable-dropdown-selected-text>{Encode(selectedSummary)}</span>
    <gcds-icon class=""fdcp-searchable-dropdown__trigger-icon"" name=""chevron-down"" size=""text"" aria-hidden=""true""></gcds-icon>
</button>");
            AppendSizer(sb, items);

            if (SelectionMode == FDCPSearchableDropdownSelectionMode.Single)
            {
                string selectedValue = selectedValues.FirstOrDefault() ?? string.Empty;
                sb.AppendLine(CultureInfo.InvariantCulture, $@"<input type=""hidden""
       name=""{EncodeAttribute(field.Name)}""
       value=""{EncodeAttribute(selectedValue)}""
       data-fdcp-searchable-dropdown-single-input />");
            }

            sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"fdcp-searchable-dropdown__panel\" id=\"{EncodeAttribute(panelId)}\" hidden data-fdcp-searchable-dropdown-panel>");
            sb.AppendLine(CultureInfo.InvariantCulture, $@"<div class=""fdcp-searchable-dropdown__search-wrapper"">
<label class=""visually-hidden"" id=""{EncodeAttribute(searchLabelId)}"" for=""{EncodeAttribute(searchId)}"">{Encode(SearchLabel)}</label>
<input type=""search""
       class=""fdcp-searchable-dropdown__search""
       id=""{EncodeAttribute(searchId)}""
       placeholder=""{EncodeAttribute(SearchPlaceholder)}""
       {searchComboboxAttributes}
       data-fdcp-searchable-dropdown-search />
<span class=""fdcp-searchable-dropdown__search-icon fa-solid fa-magnifying-glass"" aria-hidden=""true""></span>
</div>");
            string optionsRole = SelectionMode == FDCPSearchableDropdownSelectionMode.Single ? "listbox" : "group";
            sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"fdcp-searchable-dropdown__options\" id=\"{EncodeAttribute(optionsId)}\" role=\"{optionsRole}\" aria-labelledby=\"{EncodeAttribute(labelId)}\" data-fdcp-searchable-dropdown-options>");
            AppendOptions(sb, items, selectedValues, field, componentId);
            sb.AppendLine("</div>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"fdcp-searchable-dropdown__no-results\" hidden data-fdcp-searchable-dropdown-no-results>{Encode(NoResultsText)}</div>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"visually-hidden\" id=\"{EncodeAttribute(statusId)}\" aria-live=\"polite\" aria-atomic=\"true\" data-fdcp-searchable-dropdown-status></div>");

            if (!string.IsNullOrWhiteSpace(footerSlot))
            {
                sb.AppendLine("<div class=\"fdcp-searchable-dropdown__footer\" aria-live=\"polite\" aria-atomic=\"true\" data-fdcp-searchable-dropdown-footer>");
                sb.AppendLine(footerSlot);
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");

            string? errorMessage = ResolveModelStateError(field.Name);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"fdcp-searchable-dropdown__error\" id=\"{EncodeAttribute(errorId)}\">{Encode(errorMessage)}</div>");
            }

            output.Content.SetHtmlContent(sb.ToString());
        }

        private static void AppendSizer(StringBuilder sb, IEnumerable<SelectListItem> items)
        {
            sb.AppendLine("<div class=\"fdcp-searchable-dropdown__sizer\" aria-hidden=\"true\">");

            foreach (var item in items)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"<span>{Encode(item.Text)}</span>");
            }

            sb.AppendLine("</div>");
        }

        private void AppendOptions(StringBuilder sb, List<SelectListItem> items, HashSet<string> selectedValues, FormFieldContext field, string componentId)
        {
            int optionIndex = 0;
            int groupIndex = 0;

            foreach (var group in GroupItems(items))
            {
                if (!string.IsNullOrWhiteSpace(group.Name))
                {
                    string groupLabelId = SanitizeId($"{componentId}_group_{groupIndex}");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"fdcp-searchable-dropdown__group\" role=\"group\" aria-labelledby=\"{EncodeAttribute(groupLabelId)}\" data-fdcp-searchable-dropdown-group>");
                    sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"fdcp-searchable-dropdown__group-label\" id=\"{EncodeAttribute(groupLabelId)}\">{Encode(group.Name)}</div>");
                }

                foreach (var item in group.Items)
                {
                    string optionId = SanitizeId($"{componentId}_{optionIndex}_{item.Value}");
                    bool selected = selectedValues.Contains(item.Value);
                    string isChecked = selected ? " checked" : string.Empty;
                    string isSelected = selected ? " is-selected" : string.Empty;
                    string ariaSelected = selected.ToString().ToLowerInvariant();
                    string disabled = field.Disabled || item.Disabled ? " disabled" : string.Empty;
                    string ariaDisabled = (field.Disabled || item.Disabled).ToString().ToLowerInvariant();

                    if (SelectionMode == FDCPSearchableDropdownSelectionMode.Single)
                    {
                        sb.AppendLine(CultureInfo.InvariantCulture, $@"<div
        class=""fdcp-searchable-dropdown__option fdcp-searchable-dropdown__option-item{isSelected}""
        id=""{EncodeAttribute(optionId)}""
        role=""option""
        aria-selected=""{ariaSelected}""
        aria-disabled=""{ariaDisabled}""
        tabindex=""-1""
        data-fdcp-searchable-dropdown-option
        data-option-text=""{EncodeAttribute(item.Text)}""
        data-option-value=""{EncodeAttribute(item.Value)}""
        data-option-label=""{EncodeAttribute(item.Text)}"">{Encode(item.Text)}</div>");
                    }
                    else
                    {
                        sb.AppendLine(CultureInfo.InvariantCulture, $@"<div class=""fdcp-searchable-dropdown__option"" data-fdcp-searchable-dropdown-option data-option-text=""{EncodeAttribute(item.Text)}"">
    <input type=""checkbox""
           class=""fdcp-searchable-dropdown__input""
           name=""{EncodeAttribute(field.Name)}""
           id=""{EncodeAttribute(optionId)}""
           value=""{EncodeAttribute(item.Value)}""
           data-option-label=""{EncodeAttribute(item.Text)}""{isChecked}{disabled} />
    <label class=""fdcp-searchable-dropdown__option-label"" for=""{EncodeAttribute(optionId)}"">{Encode(item.Text)}</label>
</div>");
                    }

                    optionIndex++;
                }

                if (!string.IsNullOrWhiteSpace(group.Name))
                {
                    sb.AppendLine("</div>");
                }

                groupIndex++;
            }
        }

        private static IEnumerable<OptionGroup> GroupItems(IEnumerable<SelectListItem> items)
        {
            var groups = new List<OptionGroup>();

            foreach (var item in items)
            {
                string groupName = item.Group?.Name ?? string.Empty;
                var group = groups.FirstOrDefault(existing => existing.Name == groupName);
                if (group == null)
                {
                    group = new OptionGroup(groupName);
                    groups.Add(group);
                }

                group.Items.Add(item);
            }

            return groups;
        }

        private static HashSet<string> GetSelectedValues(FormFieldContext field)
        {
            if (field.Model is IEnumerable<string> values)
            {
                return values.ToHashSet(StringComparer.Ordinal);
            }

            if (!string.IsNullOrWhiteSpace(field.Value))
            {
                return field.Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToHashSet(StringComparer.Ordinal);
            }

            return new HashSet<string>(StringComparer.Ordinal);
        }

        private static async Task<string> GetSlotContentAsync(TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync().ConfigureAwait(true);
            string html = childContent.GetContent();

            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            return ExtractSlotContent(html, "selected-options") ?? ExtractSlotContent(html, "footer") ?? string.Empty;
        }

        private static string? ExtractSlotContent(string html, string slotName)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var slotNode = doc.DocumentNode
                .Descendants()
                .FirstOrDefault(node => node.Attributes["slot"]?.Value == slotName);

            return slotNode?.InnerHtml.Trim();
        }

        private static string BuildDescribedBy(string hint, string hintId, string? errorMessage, string errorId)
        {
            var ids = new List<string>();

            if (!string.IsNullOrWhiteSpace(hint))
            {
                ids.Add(hintId);
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                ids.Add(errorId);
            }

            return string.Join(" ", ids);
        }

        private static string SanitizeId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "fdcp_searchable_dropdown";
            }

            var sb = new StringBuilder(value.Length);
            foreach (char character in value)
            {
                sb.Append(char.IsLetterOrDigit(character) || character is '_' or '-' or ':' ? character : '_');
            }

            return sb.ToString();
        }

        private static string Encode(string? value)
        {
            return HtmlEncoder.Default.Encode(value ?? string.Empty);
        }

        private static string EncodeAttribute(string? value)
        {
            return HtmlEncoder.Default.Encode(value ?? string.Empty);
        }

        private sealed class OptionGroup(string name)
        {
            public string Name { get; } = name;

            public List<SelectListItem> Items { get; } = new();
        }
    }

    /// <summary>
    /// Selection behavior for the searchable dropdown.
    /// </summary>
    public enum FDCPSearchableDropdownSelectionMode
    {
        /// <summary>
        /// Allows one selected option.
        /// </summary>
        Single,

        /// <summary>
        /// Allows multiple selected options.
        /// </summary>
        Multiple
    }
}
