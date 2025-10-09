using GCFoundation.Components.Models;
using System.ComponentModel.DataAnnotations;

namespace GCFoundation.Web.Models.Template
{
    /// <summary>
    /// View model for the Stepper template demo form.
    /// </summary>
    public sealed class TemplateStepperFormViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Required]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Required]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the selected province or territory.
        /// </summary>
        [Display(Name = "Province or territory")]
        public string? Province { get; set; }

        /// <summary>
        /// Gets or sets the selected contact options.
        /// </summary>
        [Display(Name = "Contact options", Description = "Choose one or more ways we can contact you")]
        public IEnumerable<string>? Options { get; set; }
    }
}