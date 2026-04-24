using GCFoundation.Components.Enums;
using GCFoundation.Components.Models;
using GCFoundation.Components.Resources;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// A tag helper that renders a step indicator/progress component for multi-step processes.
    /// Displays numbered steps with labels and indicates completed, active, and upcoming steps.
    /// </summary>
    /// <remarks>
    /// Usage example:
    /// <code>
    /// &lt;fdcp-stepper current-step=&quot;2&quot; steps=&quot;@(new[] { new Step { StepNumber = 1, Status = StepStatus.completed }, new Step { StepNumber = 2, Status = StepStatus.InProgress }, new Step { StepNumber = 3, Status = StepStatus.NotStarted } })&quot;&gt;
    /// &lt;/fdcp-stepper&gt;
    /// </code>
    /// </remarks>
    [HtmlTargetElement("fdcp-stepper")]
    public class FDCPStepperTagHelper : TagHelper
    {
        /// <summary>
        /// Gets or sets the current active step number (1-based index).
        /// </summary>
        public int CurrentStep { get; set; } = 1;

        /// <summary>
        /// The HTML heading tag to be used (e.g., h1, h2, etc.) in the stepper's heading.
        /// Default is <see cref="HeadingTag.h2"/>.
        /// </summary>
        public HeadingTag HeadingTag { get; set; } = HeadingTag.h2;

        /// <summary>
        /// The main heading text to display in the stepper's heading.
        /// </summary>
        public string HeadingTitle { get; set; } = Stepper.Title_Default;

        /// <summary>
        /// Gets or sets the collection of steps for the process.
        /// </summary>
        public IEnumerable<StepperStep> Steps { get; set; } = new List<StepperStep>();

        /// <summary>
        /// Processes the tag helper and generates the HTML output for the stepper component.
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag.</param>
        /// <param name="output">The output that will be rendered by the tag helper.</param>
        /// <exception cref="ArgumentNullException">Thrown when output is null.</exception>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);

            output.TagName = "div";
            var html = new StringBuilder();

            // Only render visible, labeled steps and keep the output stable/deterministic.
            // This makes the component easier to reason about (and makes SR output match what is actually visible).
            var visibleSteps = Steps
                .Where(s => !s.IsHidden && !string.IsNullOrWhiteSpace(s.Label))
                .OrderBy(s => s.StepNumber)
                .ToList();

            var totalSteps = visibleSteps.Count;
            // Guard against out-of-range values so the component stays usable even with invalid input.
            var normalizedCurrentStep = totalSteps == 0 ? 1 : Math.Clamp(CurrentStep, 1, totalSteps);

            // Encode any plain-text values we output as HTML to avoid accidental injection.
            var headingTitle = HtmlEncoder.Default.Encode(HeadingTitle ?? string.Empty);
            html.AppendLine(CultureInfo.InvariantCulture, $"<gcds-heading tag='{HeadingTag}'>{headingTitle}</gcds-heading>");

            // Screen reader announcement for the current step (useful if the component updates dynamically).
            if (totalSteps > 0)
            {
                var current = visibleSteps.FirstOrDefault(s => s.StepNumber == normalizedCurrentStep) ?? visibleSteps[normalizedCurrentStep - 1];
                var currentLabel = HtmlEncoder.Default.Encode(current.Label ?? string.Empty);
                html.AppendLine(CultureInfo.InvariantCulture,
                    $"<div class='visibility-sr-only' aria-live='polite' aria-atomic='true'>{string.Format(CultureInfo.InvariantCulture, Stepper.SR_CurrentStepAnnouncement, normalizedCurrentStep, totalSteps, currentLabel)}</div>");
            }

            // Use nav + ordered list semantics so assistive tech understands this is a progress indicator with N steps.
            html.AppendLine("<nav class='fdcp-stepper' aria-label='Progress'>");
            html.AppendLine("<ol class='fdcp-stepper__list'>");

            foreach (var step in visibleSteps)
            {
                var status = step.GetStatusByCurrentStep(normalizedCurrentStep);
                var statusText = status switch
                {
                    StepperStepStatus.active => Stepper.SR_StatusCurrent,
                    StepperStepStatus.completed => Stepper.SR_StatusCompleted,
                    _ => Stepper.SR_StatusUpcoming
                };

                // Labels are plain text (rendered as text in both link and non-link states).
                var labelText = HtmlEncoder.Default.Encode(step.Label ?? string.Empty);
                var circleInnerHtml = step.GetDisplayHtml(normalizedCurrentStep);

                html.AppendLine(CultureInfo.InvariantCulture, $"<li class='fdcp-step {status}'>");

                // Interactive wrapper: only render as link when it's not the current step.
                // This avoids presenting the "current step" as a link (which is confusing for keyboard and SR users).
                var isLink = step.IsLink && !string.IsNullOrWhiteSpace(step.LinkUrl) && status != StepperStepStatus.active;
                if (isLink)
                {
                    var href = HtmlEncoder.Default.Encode(step.LinkUrl!);
                    html.AppendLine(CultureInfo.InvariantCulture, $"<a class='fdcp-step__link' href='{href}'>");
                }
                else
                {
                    html.AppendLine("<div class='fdcp-step__content'>");
                }

                // Marker (purely visual).
                // The circle content can contain decorative icons; keep it out of the accessibility tree.
                html.AppendLine(CultureInfo.InvariantCulture, $"<span class='fdcp-step-circle' aria-hidden='true'>{circleInnerHtml}</span>");

                // Step label + SR-only status.
                // aria-current="step" is the recommended way to identify the current item in a multi-step process.
                var ariaCurrent = status == StepperStepStatus.active ? " aria-current='step'" : string.Empty;
                html.AppendLine(CultureInfo.InvariantCulture, $"<span class='fdcp-step-label'{ariaCurrent}>{labelText}<span class='visibility-sr-only'> ({HtmlEncoder.Default.Encode(statusText)})</span></span>");

                // Status badge (if defined).
                if (!string.IsNullOrEmpty(step.StatusBadgeLabel))
                {
                    var badgeHtml = RenderStatusBadge(step);
                    if (!string.IsNullOrEmpty(badgeHtml))
                        html.AppendLine(badgeHtml);
                }

                html.AppendLine(isLink ? "</a>" : "</div>");
                html.AppendLine("</li>");
            }

            html.AppendLine("</ol>");
            html.AppendLine("</nav>");
            output.Content.SetHtmlContent(html.ToString());
        }

        /// <summary>
        /// Renders the status badge for a step by delegating to <see cref="FDCPBadgeTagHelper"/>.
        /// This ensures any changes to the badge tag helper are automatically reflected here.
        /// </summary>
        /// <param name="step">The step whose status badge should be rendered.</param>
        /// <returns>HTML string for the badge, or an empty string if rendering fails.</returns>
        private static string RenderStatusBadge(StepperStep step)
        {
            if (string.IsNullOrEmpty(step.StatusBadgeLabel))
                return string.Empty;

            var badgeHelper = new FDCPBadgeTagHelper
            {
                Style = step.StatusBadgeStyle,
                Inverted = step.StatusBadgeStyleInverted ?? false
            };

            var context = new TagHelperContext(
                tagName: "fdcp-badge",
                allAttributes: new TagHelperAttributeList(),
                items: new Dictionary<object, object?>(),
                uniqueId: string.Create(CultureInfo.InvariantCulture, $"fdcp-badge-{step.StepNumber}")
            );

            var childContent = new DefaultTagHelperContent();
            childContent.SetHtmlContent(step.StatusBadgeLabel);

            var output = new TagHelperOutput(
                "fdcp-badge",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) =>
                {
                    return Task.FromResult<TagHelperContent>(childContent);
                })
            {
                TagMode = TagMode.StartTagAndEndTag
            };

            badgeHelper.ProcessAsync(context, output).GetAwaiter().GetResult();

            using var writer = new StringWriter(CultureInfo.InvariantCulture);
            output.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}