using GCFoundation.Common.Utilities;
using GCFoundation.Components.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// A tag helper for rendering a rich text editor using Quill.js.
    /// Adheres to GCDS guidelines and ensures accessibility (WCAG 2.1 AAA).
    /// </summary>
    [HtmlTargetElement("fdcp-rich-text", Attributes = "for")]
    public class FDCPRichTextTagHelper : FDCPBaseFormComponentTagHelper
    {
        /// <summary>
        /// Gets or sets the toolbar configuration (Basic, Standard, Full).
        /// </summary>
        public FDCPRichTextToolbar Toolbar { get; set; } = FDCPRichTextToolbar.Basic;

        /// <summary>
        /// Gets or sets the height of the editor. Default is "200px".
        /// </summary>
        public string Height { get; set; } = "200px";

        /// <summary>
        /// Gets or sets the placeholder text.
        /// </summary>
        public string? Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the set of templates available for insertion within the editor.
        /// The dictionary key is the template name, the value is the HTML snippet.
        /// </summary>
        public IDictionary<string, string>? Templates { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            if (For == null)
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            AppendClass(output, "gcds-input-wrapper fdcp-rich-text-container gc-form-group");

            string fieldName = Name ?? For.Name;
            string fieldId = Id ?? fieldName;
            string editorId = $"{fieldId}_editor";
            string hintId = $"{fieldId}_hint";
            string labelId = $"{fieldId}_label";
            string errorId = $"{fieldId}_error";

            PropertyInfo? property = For.Metadata.ContainerType?.GetProperty(For.Metadata.PropertyName ?? string.Empty);
            string labelText = property != null ? GetLocalizedLabel(property) : fieldName;
            string hintText = property != null ? GetLocalizedHint(property) : string.Empty;

            bool required = For.Metadata.ValidatorMetadata.OfType<RequiredAttribute>().Any()
                            || PropertyInfo?.GetCustomAttribute<RequiredAttribute>() != null;
            if (IsRequired.HasValue)
            {
                required = IsRequired.Value;
            }

            // 1. Render label text container (referenced via aria-labelledby)
            var label = new TagBuilder("div");
            label.AddCssClass("fdcp-rich-text-label");
            label.Attributes.Add("id", labelId);
            label.Attributes.Add("lang", LanguageUtility.GetCurrentApplicationLanguage());

            var labelTextSpan = new TagBuilder("span");
            labelTextSpan.InnerHtml.Append(labelText);
            label.InnerHtml.AppendHtml(labelTextSpan);

            if (required)
            {
                var requiredText = GCFoundation.Components.Resources.Localization.Required;
                var requiredSpan = new TagBuilder("span");
                requiredSpan.Attributes.Add("aria-hidden", "true");
                requiredSpan.AddCssClass("label--required");
                requiredSpan.InnerHtml.Append($" ({requiredText})");
                label.InnerHtml.AppendHtml(requiredSpan);
            }

            output.Content.AppendHtml(label);
            
            // 2. Render Hint (if any) - Typically GCDS puts hint inside or after label, 
            // but for custom rich text we'll put it after label.
            if (!string.IsNullOrEmpty(hintText))
            {
                var hintBuilder = new TagBuilder("p");
                hintBuilder.AddCssClass("gcds-hint");
                hintBuilder.Attributes.Add("id", hintId);
                hintBuilder.InnerHtml.Append(hintText);
                output.Content.AppendHtml(hintBuilder);
            }

            var editorBuilder = new TagBuilder("div");
            editorBuilder.Attributes.Add("id", editorId);
            editorBuilder.AddCssClass("fdcp-rich-text-editor");
            editorBuilder.Attributes.Add("data-fdcp-rich-text", "true");
            editorBuilder.Attributes.Add("data-for", fieldId);
            editorBuilder.Attributes.Add("data-toolbar", Toolbar.ToString().ToLowerInvariant());
            editorBuilder.Attributes.Add("style", $"height: {Height};");
            editorBuilder.Attributes.Add("role", "textbox");
            editorBuilder.Attributes.Add("aria-multiline", "true");
            editorBuilder.Attributes.Add("lang", LanguageUtility.GetCurrentApplicationLanguage());
            editorBuilder.Attributes.Add("aria-labelledby", labelId);

            if (!string.IsNullOrEmpty(Placeholder))
            {
                editorBuilder.Attributes.Add("data-placeholder", Placeholder);
            }

            if (Templates?.Any() == true)
            {
                string templatesJson = JsonSerializer.Serialize(Templates);
                editorBuilder.Attributes.Add("data-templates", templatesJson);
            }

            var describedByIds = new List<string>();
            if (!string.IsNullOrEmpty(hintText))
            {
                describedByIds.Add(hintId);
            }

            var wrapperBuilder = new TagBuilder("div");
            wrapperBuilder.AddCssClass("fdcp-rich-text-wrapper");
            wrapperBuilder.InnerHtml.AppendHtml(editorBuilder);
            output.Content.AppendHtml(wrapperBuilder);

            var inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes.Add("type", "hidden");
            inputBuilder.Attributes.Add("id", fieldId);
            inputBuilder.Attributes.Add("name", fieldName);
            inputBuilder.Attributes.Add("lang", LanguageUtility.GetCurrentApplicationLanguage());
            inputBuilder.Attributes.Add("aria-hidden", "true");
            if (required)
            {
                inputBuilder.Attributes.Add("required", "required");
            }

            var value = For.Model?.ToString();
            if (!string.IsNullOrEmpty(value))
            {
                inputBuilder.Attributes.Add("value", value);
            }
            output.Content.AppendHtml(inputBuilder);

            bool hasError = ViewContext?.ModelState?.ContainsKey(fieldName) == true &&
                            ViewContext.ModelState[fieldName]?.Errors?.Count > 0;
            if (hasError)
            {
                string errorMessage = ViewContext!.ModelState[fieldName]!.Errors[0].ErrorMessage;
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    var errorBuilder = new TagBuilder("gcds-error-message");
                    errorBuilder.Attributes.Add("message-id", errorId);
                    errorBuilder.Attributes.Add("id", errorId);
                    errorBuilder.InnerHtml.Append(errorMessage);
                    output.Content.AppendHtml(errorBuilder);

                    editorBuilder.Attributes.Add("aria-invalid", "true");
                    describedByIds.Add(errorId);
                }
            }

            if (describedByIds.Count > 0)
            {
                editorBuilder.Attributes.Add("aria-describedby", string.Join(' ', describedByIds));
            }
        }
        private static void AppendClass(TagHelperOutput output, string classNames)
        {
            if (output.Attributes.TryGetAttribute("class", out var existing))
            {
                var merged = string.IsNullOrWhiteSpace(existing.Value?.ToString())
                    ? classNames
                    : $"{existing.Value} {classNames}";
                output.Attributes.SetAttribute("class", merged.Trim());
            }
            else
            {
                output.Attributes.SetAttribute("class", classNames);
            }
        }
    }
}


