using GCFoundation.Components.Enums;
using GCFoundation.Components.Resources;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders a standalone modal component.
    /// Use &lt;fdcp-modal&gt; in your Razor views to generate a modal dialog.
    /// </summary>
    [HtmlTargetElement("fdcp-modal")]
    public class FDCPModalTagHelper(IStringLocalizer<Modal> localizer) : TagHelper
    {
        private readonly IStringLocalizer<Modal> _localizer = localizer;
        /// <summary>
        /// Determines if a close ("×") button is shown in the modal header.
        /// </summary>
        public bool HideCloseButton { get; set; }

        /// <summary>
        /// The ID of the modal element. Must be unique on the page.
        /// </summary>
        public string ModalId { get; set; } = default!;

        /// <summary>
        ///  Sets whether the modal is scrollable. Defaults to true.
        /// </summary>
        public bool Scrollable { get; set; }

        /// <summary>
        /// Sets the size of the modal. sm, md, or lg.
        /// </summary>
        public ModalSize Size { get; set; } = ModalSize.regular;

        /// <summary>
        /// Sets the visual/semantic state of the modal. Default, Info, or Warning.
        /// </summary>
        public ModalState State { get; set; } = ModalState.regular;

        /// <summary>
        /// Sets whether the modal will have a static backdrop (prevents closing by clicking outside the modal).
        /// </summary>
        public bool StaticBackdrop { get; set; }

        /// <summary>
        /// The title displayed in the modal header.
        /// </summary>
        public string Title { get; set; } = string.Empty;


        /// <inheritdoc/>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "modal-overlay");
            output.Attributes.SetAttribute("modal-id", ModalId);
            output.Attributes.SetAttribute("tabindex", "-1");
            output.Attributes.SetAttribute("aria-labelledby", $"{ModalId}Label");
            output.Attributes.SetAttribute("aria-describedby", $"{ModalId}Description");
            output.Attributes.SetAttribute("aria-hidden", "true");
            output.Attributes.SetAttribute("role", "dialog");

            if (StaticBackdrop)
            {
                output.Attributes.SetAttribute("data-static", "true");
            }

            var dialogClasses = new List<string> { "modal-overlay__dialog" };
            if (Scrollable) dialogClasses.Add("modal-overlay__dialog--scrollable");
            if (Size == ModalSize.small) dialogClasses.Add("container-sm");
            else if (Size == ModalSize.large) dialogClasses.Add("container-xl");

            var variant = State.ToString().ToLowerInvariant();
            var modalClasses = new List<string> { "modal" };
            if (State != ModalState.regular)
            {
                modalClasses.Add($"modal--{variant}");
            }

            var childContentRaw = (await output.GetChildContentAsync().ConfigureAwait(true)).GetContent();
            var bodySlot = ExtractSlotContent(childContentRaw, "body");
            var footerSlot = ExtractSlotContent(childContentRaw, "footer");
            var cleanedContent = RemoveSlotElements(childContentRaw);

            var bodyClasses = "modal__body" + (Scrollable ? " modal__body--scrollable" : "");

            var sb = new StringBuilder();
            sb.AppendLine("<div class=\"modal-overlay__backdrop\"></div>");
            sb.AppendLine(CultureInfo.InvariantCulture, $"<div class=\"{string.Join(" ", dialogClasses)}\" role=\"document\">");
            sb.AppendLine(CultureInfo.InvariantCulture, $"  <div class=\"{string.Join(" ", modalClasses)}\">");
            sb.Append(BuildHeader(variant));
            sb.AppendLine(CultureInfo.InvariantCulture, $"    <div id=\"{ModalId}Description\" class=\"{bodyClasses}\" tabindex=\"0\">");
            sb.AppendLine(!string.IsNullOrWhiteSpace(bodySlot) ? bodySlot : cleanedContent.Trim());
            sb.AppendLine("    </div>");
            if (!string.IsNullOrWhiteSpace(footerSlot))
            {
                sb.AppendLine("    <hr />");
                sb.AppendLine("    <div class=\"modal__footer\">");
                sb.AppendLine(footerSlot);
                sb.AppendLine("    </div>");
            }
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            output.Content.SetHtmlContent(sb.ToString());
        }

        private string BuildHeader(string variant)
        {
            var closeText = _localizer["Modal_Close"];
            string CloseButton() =>
                !HideCloseButton
                    ? $"<gcds-button button-role=\"secondary\" size=\"small\" class=\"fdcp-modal-close modal__close--{variant}\">{closeText}</gcds-button>"
                    : string.Empty;

            switch (variant)
            {
                case "warning": 
                    return $@"
                        <div class=""modal__header bg-warning"">
                            <div class=""modal__title-group"">
                                <gcds-icon name=""warning-triangle""></gcds-icon>
                                <p class=""font-h5 mb-0"" id=""{ModalId}Label"">{Title}</p>
                            </div>
                            {CloseButton()}
                        </div>
                    ";

                case "info":
                    return  $@"
                        <div class=""modal__header bg-info"">
                            <div class=""modal__title-group text-light"">
                                <gcds-icon name=""info-circle"" class=""text-current""></gcds-icon>
                                <p class=""font-h5 text-current mb-0"" id=""{ModalId}Label"">{Title}</p>
                            </div>
                            {CloseButton()}
                        </div>
                    "; 
                default:
                    return $@"
                        <div class=""modal__header bg-primary"">
                            <p class=""font-h5 text-light mb-0"" id=""{ModalId}Label"">{Title}</p>
                            {CloseButton()}
                        </div>
                    ";
                }
        }


        /// <summary>
        /// Extracts the inner HTML content of an element with the specified slot name.
        /// </summary>
        private static string ExtractSlotContent(string html, string slotName)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var slotNode = doc.DocumentNode
                .Descendants()
                .FirstOrDefault(n => n.Attributes["slot"]?.Value == slotName);

            return slotNode?.InnerHtml.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Removes all slot-marked elements from the HTML string.
        /// </summary>
        private static string RemoveSlotElements(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodesToRemove = doc.DocumentNode
                .Descendants()
                .Where(n => n.Attributes["slot"] != null)
                .ToList();

            foreach (var node in nodesToRemove)
            {
                node.ParentNode.RemoveChild(node, keepGrandChildren: false);
            }

            return doc.DocumentNode.InnerHtml.Trim();
        }
    }
}