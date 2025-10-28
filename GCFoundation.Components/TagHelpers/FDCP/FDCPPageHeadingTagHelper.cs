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
        public PageHeadingSize Size { get; set; } = PageHeadingSize.regular;

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

            var articleClass = "text-container";
            var containerClass = "fdcp-page-heading-container";
            var pageHeadingBgClass = "fdcp-page-heading-bg";

            if (!string.IsNullOrWhiteSpace(Src))
            {
                containerClass += " fdcp-page-heading-has-bg";
                output.Attributes.SetAttribute("data-bg-src", Src);
            }

            switch (Size)
            {
                case PageHeadingSize.compact:
                    articleClass += " sm:fdcp-py-350 fdcp-py-200 xl:fdcp-ps-0 sm:fdcp-ps-600 fdcp-ps-450 sm:fdcp-pe-750 fdcp-pe-450";
                    containerClass += " fdcp-page-heading-compact";
                    pageHeadingBgClass += " md:fdcp-py-500 fdcp-py-250";
                    break;
                case PageHeadingSize.large:
                    articleClass += " sm:fdcp-py-750 fdcp-py-450 xl:fdcp-ps-0 sm:fdcp-ps-600 fdcp-ps-450 sm:fdcp-pe-750 fdcp-pe-450";
                    containerClass += " fdcp-page-heading-large";
                    pageHeadingBgClass += " md:fdcp-py-1250 fdcp-py-900";
                    break;
                case PageHeadingSize.regular:
                default:
                    articleClass += " sm:fdcp-py-600 fdcp-py-300 xl:fdcp-ps-0 sm:fdcp-ps-600 fdcp-ps-450 sm:fdcp-pe-750 fdcp-pe-450";
                    pageHeadingBgClass += " md:fdcp-py-900 fdcp-py-600";
                    break;
            }

            output.Attributes.SetAttribute("class", containerClass);

            var content = new StringBuilder();

            content.AppendLine(CultureInfo.InvariantCulture, $"<div class='{pageHeadingBgClass}'>");
            content.AppendLine(CultureInfo.InvariantCulture, $"<div class='container-xl mx-auto'>");

            if (TextEmphasis)
            {
                switch (BackgroundColour)
                {
                    case BackgroundColour.dark:
                        articleClass += " fdcp-bg-dark";
                        break;
                    case BackgroundColour.light:
                        articleClass += " fdcp-bg-light";
                        break;
                    case BackgroundColour.white:
                        articleClass += " fdcp-bg-white";
                        break;
                    case BackgroundColour.primary:
                    default:
                        articleClass += " fdcp-bg-primary";
                        break;
                }
            }
            switch (TextColour)
            {
                case TextColour.primary:
                    articleClass += " fdcp-text-primary";
                    break;
                case TextColour.secondary:
                    articleClass += " fdcp-text-secondary";
                    break;
                case TextColour.light:
                default:
                    articleClass += " fdcp-text-light";
                    break;
            }
            content.AppendLine(CultureInfo.InvariantCulture, $"<article class='{articleClass}'>");
            content.AppendLine(CultureInfo.InvariantCulture, $"<gcds-heading tag='h1'>{Title}</gcds-heading>");

            if (!string.IsNullOrWhiteSpace(Description))
            {
                var descriptionTextRole = string.Empty;
                switch (TextColour)
                {
                    case TextColour.primary:
                        descriptionTextRole = " text-role='primary'";
                        break;
                    case TextColour.secondary:
                        descriptionTextRole = " text-role='secondary'";
                        break;
                    case TextColour.light:
                    default:
                        descriptionTextRole = " text-role='light'";
                        break;
                }
                content.AppendLine(CultureInfo.InvariantCulture, $"<gcds-text{descriptionTextRole}>{Description}</gcds-text>");
            }

            content.AppendLine("</article>");
            content.AppendLine("</div>");
            content.AppendLine("</div>");

            output.Content.SetHtmlContent(content.ToString());
        }
    }
}
