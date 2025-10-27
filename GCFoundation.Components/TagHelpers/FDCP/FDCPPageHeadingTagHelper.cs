using GCFoundation.Components.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Text;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// TagHelper for rendering a GC-style page header with title, description, and optional background image.
    /// </summary>
    [HtmlTargetElement("fdcp-page-heading")]
    public class FDCPPageHeadingTagHelper : TagHelper
    {
        /// <summary>
        /// The main heading text to display in the page header.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Sets the colour of the background of the text container to emphasize the content.
        /// </summary>
        public BackgroundColour BackgroundColour { get; set; } = BackgroundColour.primary;

        /// <summary>
        /// The description text displayed below the title.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Sets the size of the page header. Default, or Large.
        /// </summary>
        public PageHeadingSize Size { get; set; } = PageHeadingSize.Default;

        /// <summary>
        /// The URL of the background image for the page header.
        /// </summary>
        public string? Src { get; set; }

        /// <summary>
        /// Sets the colour of the text content.
        /// </summary>
        public TextColour TextColour { get; set; } = TextColour.light;

        /// <summary>
        /// Adds a light background and a border around the text container to emphasize the content.
        /// </summary>
        public bool TextEmphasis { get; set; }

        /// <summary>
        /// Processes the tag helper and renders the page header markup.
        /// </summary>
        /// <param name="context">The context for the tag helper.</param>
        /// <param name="output">The output for the tag helper.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            var classValue = "fdcp-page-heading-container";
            if (!string.IsNullOrWhiteSpace(Src))
            {
                classValue += " fdcp-page-heading-has-bg";
                output.Attributes.SetAttribute("data-bg-src", Src);
            }

            switch (Size)
            {
                case PageHeadingSize.Compact:
                    classValue += " fdcp-page-heading-compact";
                    break;
                case PageHeadingSize.Large:
                    classValue += " fdcp-page-heading-large";
                    break;
                case PageHeadingSize.Default:
                default:
                    break;
            }

            output.Attributes.SetAttribute("class", classValue);

            var content = new StringBuilder();

            content.AppendLine(CultureInfo.InvariantCulture, $"<div class='md:fdcp-py-1250 fdcp-py-900 fdcp-page-heading-bg'>");
            content.AppendLine(CultureInfo.InvariantCulture, $"<div class='container-xl mx-auto'>");

            var textContainerClass = "sm:fdcp-py-750 fdcp-py-450 xl:fdcp-ps-0 sm:fdcp-ps-600 fdcp-ps-450 sm:fdcp-pe-750 fdcp-pe-300 text-container";
            if (TextEmphasis)
            {
                switch (BackgroundColour)
                {
                    case BackgroundColour.dark:
                        textContainerClass += " fdcp-bg-dark";
                        break;
                    case BackgroundColour.light:
                        textContainerClass += " fdcp-bg-light";
                        break;
                    case BackgroundColour.white:
                        textContainerClass += " fdcp-bg-white";
                        break;
                    case BackgroundColour.primary:
                    default:
                        textContainerClass += " fdcp-bg-primary";
                        break;
                }
            }
            switch (TextColour)
            {
                case TextColour.primary:
                    textContainerClass += " fdcp-text-primary";
                    break;
                case TextColour.secondary:
                    textContainerClass += " fdcp-text-secondary";
                    break;
                case TextColour.light:
                default:
                    textContainerClass += " fdcp-text-light";
                    break;
            }
            content.AppendLine(CultureInfo.InvariantCulture, $"<article class='{textContainerClass}'>");
            content.AppendLine(CultureInfo.InvariantCulture, $"<gcds-heading tag='h1'>{Title}</gcds-heading>");

            if (!string.IsNullOrWhiteSpace(Description))
            {
                var descriptionTextRole = string.Empty;
                if (TextEmphasis)
                    descriptionTextRole = " text-role='light'";
                content.AppendLine(CultureInfo.InvariantCulture, $"<gcds-text{descriptionTextRole}>{Description}</gcds-text>");
            }

            content.AppendLine("</article>");
            content.AppendLine("</div>");
            content.AppendLine("</div>");

            output.Content.SetHtmlContent(content.ToString());
        }
    }
}
