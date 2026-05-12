using GCFoundation.Components.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GCFoundation.Web.Models.Template
{
    /// <summary>
    /// View model for the Stepper template demo form.
    /// </summary>
    public sealed class TemplateStepperFormViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the current step in the demo. This is round-tripped through a hidden field
        /// so the server can re-render the same progress context after each post.
        /// </summary>
        public int CurrentStep { get; set; } = 1;

        /// <summary>
        /// Total number of steps in the demo Stepper. The controller uses this to clamp navigation
        /// and the component uses it to announce accurate progress to assistive technology.
        /// </summary>
        public int TotalSteps { get; } = 5;

        /// <summary>
        /// Gets or sets the selected contact options.
        /// </summary>
        [Display(Name = "Stepper_Demo_ContactOptions_Label", Description = "Stepper_Demo_ContactOptions_Hint", ResourceType = typeof(Resources.Template))]
        public IEnumerable<string>? ContactOptions { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Required]
        [Display(Name = "Stepper_Demo_FirstName_Label", Description = "Stepper_Demo_FirstName_Hint", ResourceType = typeof(Resources.Template))]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Required]
        [Display(Name = "Stepper_Demo_LastName_Label", Description = "Stepper_Demo_LastName_Hint", ResourceType = typeof(Resources.Template))]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the selected province or territory.
        /// </summary>
        [Display(Name = "Stepper_Demo_Province_Label", ResourceType = typeof(Resources.Template))]
        public string? Province { get; set; }

        /// <summary>
        /// Available options.
        /// </summary>
        public IEnumerable<SelectListItem> ContactOptionsList { get; set; } =
        [
            new() { Value = "email", Text = Resources.Template.Stepper_Demo_ContactOptions_Email },
            new() { Value = "sms", Text = Resources.Template.Stepper_Demo_ContactOptions_Sms }
        ];

        /// <summary>
        /// Available province options.
        /// </summary>
        public IEnumerable<SelectListItem> ProvinceList { get; set; } =
        [
            new() { Value = "AB", Text = Resources.Template.Stepper_Demo_Province_Alberta },
            new() { Value = "BC", Text = Resources.Template.Stepper_Demo_Province_BritishColumbia },
            new() { Value = "ON", Text = Resources.Template.Stepper_Demo_Province_Ontario },
            new() { Value = "QC", Text = Resources.Template.Stepper_Demo_Province_Quebec }
        ];
    }
}