using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders an <c>&lt;fdcp-accordion&gt;</c> element as a <c>&lt;div class="fdcp-accordion"&gt;</c>
    /// container with an injected "Open all / Close all" toggle button. Works together with the
    /// <c>FDCPAccordion</c> JavaScript class, which binds to elements with the <c>fdcp-accordion</c>
    /// class to provide single-open-at-a-time behaviour for child <c>gcds-details</c> elements,
    /// plus bulk open/close via the toggle button.
    /// </summary>
    [HtmlTargetElement("fdcp-accordion", Attributes = "accordion-id")]
    public class FdcpAccordionTagHelper : TagHelper
    {
        /// <summary>
        /// The unique identifier applied to the rendered accordion container. Used by
        /// <c>FDCPAccordion</c> (JavaScript) to bind open/close behaviour and by the
        /// injected toggle button to control all panels within this accordion.
        /// </summary>
        public string AccordionId { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var existingClass = output.Attributes["class"]?.Value?.ToString();
            var mergedClass = string.IsNullOrWhiteSpace(existingClass)
                ? "fdcp-accordion"
                : $"fdcp-accordion {existingClass}";

            output.Attributes.SetAttribute("class", mergedClass);
            output.Attributes.SetAttribute("id", AccordionId);

            var toggleButton = @"<gcds-button
                button-role=""secondary""
                class=""fdcp-accordion-toggle mb-150"">
            </gcds-button>";

            output.PreContent.SetHtmlContent(toggleButton);
        }
    }
}
