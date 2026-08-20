using GCFoundation.Common.Utilities;
using GCFoundation.Components.Attributes;
using GCFoundation.Components.Attributes.Table;
using GCFoundation.Components.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security;

namespace GCFoundation.Web.Models.Template.Crud
{
    public class EmployeeViewModel : BaseViewModel
    {
        [Display(Name = "Crud_Demo_EmployeeId_Label", ResourceType = typeof(Resources.Template))]
        [TableColumnDefinition(RowHeader = true, Sort = true)]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_EmployeeName_Label", ResourceType = typeof(Resources.Template))]
        [TableColumnDefinition(Sort = true)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_EmployeeLevel_Label", ResourceType = typeof(Resources.Template))]
        [TableColumnDefinition(Sort = true)]
        public string Level { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_Salary_Label", ResourceType = typeof(Resources.Template))]
        public double Salary { get; set; }
        public string DepartmentEn { get; set; } = string.Empty;
        public string DepartmentFr { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_Department_Label", ResourceType = typeof(Resources.Template))]
        public string Department => LanguageUtility.IsFrench()? DepartmentFr : DepartmentEn;

        [Display(Name = "Crud_Demo_ManagerName_Label", ResourceType = typeof(Resources.Template))]
        public string ManagerName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DateFormat("yyyy-mm-dd")]
        [Display(Name = "Crud_Demo_StartDate_Label", ResourceType = typeof(Resources.Template))]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [DateFormat("yyyy-mm-dd")]
        [Display(Name = "Crud_Demo_DateOfBirth_Label", ResourceType = typeof(Resources.Template))]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Display(Name = "Crud_Demo_Address_Label", ResourceType = typeof(Resources.Template))]
        public string Address { get; set; } = string.Empty;

        [TableColumnDefinition(Slotted = true)]
        public string Actions { get; set; } = string.Empty;
    }
}
