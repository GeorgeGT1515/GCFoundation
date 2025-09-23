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

            /*
            <section>
                <div class="md:py-1250 py-900"> (.my-hero-class) // image background
                    <div class="container-xl mx-auto">
                        <article class="sm:py-750 py-450 xl:ps-0 sm:ps-600 ps-450 sm:pe-750 pe-300">
                            (.my-hero-class .my-hero-content = position: relative; width: 80%; max-width: 40.625rem;)
                            (bg-primary text-light) // Dependent on selection
                            <hx>Title
                            <content>
                    
             */
            /*
            <fdcp-page-heading class="fdcp-page-heading-container fdcp-page-heading-has-bg(?) fdcp-page-heading-large(?)">
                <div class="md:py-1250 py-900 fdcp-page-heading-bg">
                    <div class="container-xl mx-auto">
                        <article class="sm:py-750 py-450 xl:ps-0 sm:ps-600 ps-450 sm:pe-750 pe-300">
                            x. (.my-hero-class .my-hero-content = "position: relative; width: 80%; max-width: 40.625rem;")
                            ?. (bg-primary text-light) // Dependent on selection
                            x. <hx>Title
                            x. <content>
             */

            var classValue = "fdcp-page-heading-container";
            if (!string.IsNullOrWhiteSpace(Src))
            {
                classValue += " fdcp-page-heading-has-bg";
                output.Attributes.SetAttribute("data-bg-src", Src);
            }

            switch (Size)
            {
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
                textContainerClass += " fdcp-bg-primary fdcp-text-light";
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
