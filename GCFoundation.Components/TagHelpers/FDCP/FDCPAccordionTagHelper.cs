using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    [HtmlTargetElement("fdcp-accordion", Attributes = "accordion-id")]
    public class FdcpAccordionTagHelper : TagHelper
    {
        [HtmlAttributeName("accordion-id")]
        public string AccordionId { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
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
                size=""small""
                class=""fdcp-accordion-toggle mb-150"">
            </gcds-button>";

            output.PreContent.SetHtmlContent(toggleButton);
        }
    }
}
