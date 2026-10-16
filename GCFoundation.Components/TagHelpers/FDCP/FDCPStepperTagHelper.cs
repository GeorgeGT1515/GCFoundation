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
        /// When not set, the active step's <see cref="StepperStep.Label"/> is used when steps are provided;
        /// otherwise <see cref="Stepper.Title_Default"/> is shown.
        /// </summary>
        public string? HeadingTitle { get; set; }

        /// <summary>
        /// Optional id applied to the rendered heading so callers can move focus to it after navigation.
        /// </summary>
        public string? HeadingId { get; set; }

        /// <summary>
        /// Gets or sets the collection of steps for the process.
        /// </summary>
        public IEnumerable<StepperStep> Steps { get; set; } = new List<StepperStep>();

        /// <summary>
        /// Controls whether step links are part of the natural keyboard tab order.
        /// When <c>true</c> (default), step links can be tabbed to like any other anchor.
        /// When <c>false</c>, step links are excluded from the tab order (<c>tabindex="-1"</c>)
        /// so that Tab from the active step moves focus directly to the next focusable element
        /// after the stepper (e.g. the first form input). Step links remain clickable.
        /// </summary>
        public bool LinksTabbable { get; set; } = true;

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
            var visibleSteps = Steps
                .Where(s => !s.IsHidden && !string.IsNullOrWhiteSpace(s.Label))
                .OrderBy(s => s.StepNumber)
                .ToList();

            var totalSteps = visibleSteps.Count;
            var normalizedCurrentStep = totalSteps == 0 ? 1 : Math.Clamp(CurrentStep, 1, totalSteps);
            var currentStep = totalSteps > 0
                ? visibleSteps.FirstOrDefault(s => s.StepNumber == normalizedCurrentStep) ?? visibleSteps[normalizedCurrentStep - 1]
                : null;
            var headingTitleText = ResolveHeadingTitle(context, currentStep);
            var headingTitle = HtmlEncoder.Default.Encode(headingTitleText);
            var headingIdAttribute = !string.IsNullOrWhiteSpace(HeadingId)
                ? $" id='{HtmlEncoder.Default.Encode(HeadingId)}' tabindex='-1'"
                : string.Empty;
            html.AppendLine(CultureInfo.InvariantCulture, $"<gcds-heading{headingIdAttribute} tag='{HeadingTag}'>{headingTitle}</gcds-heading>");

            html.AppendLine(CultureInfo.InvariantCulture, $"<nav class='fdcp-stepper' aria-label='{HtmlEncoder.Default.Encode(Stepper.SR_ProgressLabel)}'>");
            html.AppendLine("<ol class='fdcp-stepper__list' role='list'>");

            foreach (var step in visibleSteps)
            {
                var status = step.GetStatusByCurrentStep(normalizedCurrentStep);
                var labelText = HtmlEncoder.Default.Encode(step.Label ?? string.Empty);
                // The visually hidden summary is the only accessible name source for each step.
                // The active step relies on aria-current='step' instead of a "(Current step)" suffix,
                // so screen readers do not say "current" twice.
                var stepSummaryText = status == StepperStepStatus.active
                    ? string.Format(
                        CultureInfo.InvariantCulture,
                        Stepper.SR_CurrentStepAnnouncement,
                        step.StepNumber,
                        totalSteps,
                        step.Label ?? string.Empty)
                    : string.Format(
                        CultureInfo.InvariantCulture,
                        Stepper.SR_StepSummary,
                        step.StepNumber,
                        totalSteps,
                        step.Label ?? string.Empty,
                        status == StepperStepStatus.completed ? Stepper.SR_StatusCompleted : Stepper.SR_StatusUpcoming);
                var stepSummary = HtmlEncoder.Default.Encode(stepSummaryText);
                var circleInnerHtml = step.GetDisplayHtml(normalizedCurrentStep);

                html.AppendLine(CultureInfo.InvariantCulture, $"<li class='fdcp-step {status}'>");

                var isLink = step.IsLink && !string.IsNullOrWhiteSpace(step.LinkUrl) && status != StepperStepStatus.active;
                if (isLink)
                {
                    var href = HtmlEncoder.Default.Encode(step.LinkUrl!);
                    // When LinksTabbable is false, exclude step links from the natural tab order so that
                    // Tab from the active step lands on the first form input instead of the next step link.
                    var linkTabIndexAttribute = LinksTabbable ? string.Empty : " tabindex='-1'";
                    html.AppendLine(CultureInfo.InvariantCulture, $"<a class='fdcp-step__link' href='{href}' data-stepper-focus-trigger='true'{linkTabIndexAttribute}>");
                }
                else
                {
                    // The active step is focusable so Next/Previous scripts can move focus here; its name comes
                    // from the hidden summary text, and aria-current='step' conveys that it is the current step.
                    var activeAttributes = status == StepperStepStatus.active
                        ? " tabindex='0' data-stepper-active-step='true' aria-current='step'"
                        : string.Empty;
                    html.AppendLine(CultureInfo.InvariantCulture, $"<div class='fdcp-step__content'{activeAttributes}>");
                }

                html.AppendLine(CultureInfo.InvariantCulture, $"<span class='visibility-sr-only'>{stepSummary}</span>");
                html.AppendLine(CultureInfo.InvariantCulture, $"<span class='fdcp-step-circle' aria-hidden='true'>{circleInnerHtml}</span>");
                html.AppendLine(CultureInfo.InvariantCulture, $"<span class='fdcp-step-label' aria-hidden='true'>{labelText}</span>");

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
        /// Resolves the visible stepper heading. When <c>heading-title</c> is not supplied on the element,
        /// the active step label is used (Intro, Info, etc.). Explicit <c>heading-title</c> always wins.
        /// </summary>
        private string ResolveHeadingTitle(TagHelperContext context, StepperStep? currentStep)
        {
            if (context.AllAttributes.ContainsName("heading-title"))
            {
                return string.IsNullOrWhiteSpace(HeadingTitle) ? Stepper.Title_Default : HeadingTitle!;
            }

            return !string.IsNullOrWhiteSpace(currentStep?.Label)
                ? currentStep.Label
                : Stepper.Title_Default;
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
            // Preserve caller-provided HTML so badges can include SR-only context (for example, explaining why a step is skipped).
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