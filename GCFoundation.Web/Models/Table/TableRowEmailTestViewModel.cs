using GCFoundation.Components.Attributes;
using GCFoundation.Components.Attributes.Table;
using System.ComponentModel.DataAnnotations;

namespace GCFoundation.Web.Models.Table
{
    /// <summary>
    /// Sample row model for demonstrating how to render a slotted email link using
    /// <c>data-bind</c> and <c>data-bind-template-href</c> inside a cell template.
    /// </summary>
    public class TableRowEmailTestViewModel
    {
        /// <summary>
        /// The submission's unique identifier. Hidden from the rendered table columns.
        /// </summary>
        [DataType(DataType.Text)]
        [TableColumnDefinition(IsHidden = true)]
        public string SubmissionId { get; set; } = string.Empty;

        /// <summary>
        /// The name of the person who made the submission. Rendered as the row header.
        /// </summary>
        [DataType(DataType.Text)]
        [Display(Name = "Table_Submitter_Name_Header", ResourceType = typeof(Resources.Components))]
        [TableColumnDefinition(RowHeader = true)]
        public string SubmitterName { get; set; } = string.Empty;

        /// <summary>
        /// The date and time the submission was made. Formatted as <c>yyyy-MM-dd HH:mm</c>.
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Table_Date_Submitted_Header", ResourceType = typeof(Resources.Components))]
        [DateFormat("yyyy-MM-dd HH:mm")]
        public DateTime DateSubmitted { get; set; }

        /// <summary>
        /// The name of the reviewer assigned to the submission.
        /// </summary>
        [DataType(DataType.Text)]
        [Display(Name = "Table_Assigned_Reviewer_Header", ResourceType = typeof(Resources.Components))]
        public string AssignedReviewer { get; set; } = string.Empty;

        /// <summary>
        /// The submitter's email address. Rendered as a slotted <c>gcds-link</c> with a
        /// <c>mailto:</c> href built via <c>data-bind-template-href</c>.
        /// </summary>
        [DataType(DataType.Text)]
        [Display(Name = "Table_Email_Header", ResourceType = typeof(Resources.Components))]
        [TableColumnDefinition(Slotted = true)]
        public string Email { get; set; } = string.Empty;
    }
}
