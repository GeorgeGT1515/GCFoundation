using GCFoundation.Components.Attributes;
using System.ComponentModel.DataAnnotations;

namespace GCFoundation.Web.Models
{
    public class TableRowTestViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Table_Submission_Id_Header", ResourceType = typeof(Resources.Components))]
        public string SubmissionId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Table_Submitter_Name_Header", ResourceType = typeof(Resources.Components))]
        public string SubmitterName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Table_Date_Submitted_Header", ResourceType = typeof(Resources.Components))]
        [DateFormat("full")]
        public DateTime DateSubmitted { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Table_Assigned_Reviewer_Header", ResourceType = typeof(Resources.Components))]
        public string AssignedReviewer {  get; set; }
    }
}
