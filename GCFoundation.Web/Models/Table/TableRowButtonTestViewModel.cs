using GCFoundation.Components.Attributes;
using GCFoundation.Components.Attributes.Table;
using System.ComponentModel.DataAnnotations;

namespace GCFoundation.Web.Models.Table
{
    /// <summary>
    /// Sample row model used to demo binding a row field to a button attribute using
    /// <c>data-bind-{attribute}</c> inside a slotted cell template.
    /// </summary>
    public class TableRowButtonTestViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Table_Submission_Id_Header", ResourceType = typeof(Resources.Components))]
        [TableColumnDefinition(IsHidden = true)]
        public string SubmissionId { get; set; } = string.Empty;

        [DataType(DataType.Text)]
        [Display(Name = "Table_Submitter_Name_Header", ResourceType = typeof(Resources.Components))]
        [TableColumnDefinition(RowHeader = true)]
        public string SubmitterName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Table_Date_Submitted_Header", ResourceType = typeof(Resources.Components))]
        [DateFormat("yyyy-MM-dd HH:mm")]
        public DateTime DateSubmitted { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Table_Assigned_Reviewer_Header", ResourceType = typeof(Resources.Components))]
        public string AssignedReviewer { get; set; } = string.Empty;

        [Display(Name = "Table_Actions_Header", ResourceType = typeof(Resources.Components))]
        [TableColumnDefinition(Slotted = true)]
        public string Action { get; set; } = string.Empty;
    }
}
