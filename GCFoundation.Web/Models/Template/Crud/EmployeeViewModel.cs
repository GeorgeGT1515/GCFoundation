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
        [TableColumnDefinition(Slotted = true, Order = 4)]
        public string Actions { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_Address_Label", ResourceType = typeof(Resources.Template), Order = 9)]
        public string Address { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DateFormat("yyyy-mm-dd")]
        [Display(Name = "Crud_Demo_DateOfBirth_Label", ResourceType = typeof(Resources.Template), Order = 8)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Display(Name = "Crud_Demo_Department_Label", ResourceType = typeof(Resources.Template), Order = 5)]
        public string Department => LanguageUtility.IsFrench() ? DepartmentFr : DepartmentEn;

        public string DepartmentEn { get; set; } = string.Empty;

        public string DepartmentFr { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_EmployeeId_Label", ResourceType = typeof(Resources.Template), Order = 2)]
        [TableColumnDefinition(RowHeader = true, Sort = true, Order = 1)]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_EmployeeLevel_Label", ResourceType = typeof(Resources.Template), Order = 3)]
        [TableColumnDefinition(Sort = true, Order = 3)]
        public string Level { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_ManagerName_Label", ResourceType = typeof(Resources.Template), Order = 6)]
        public string ManagerName { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_EmployeeName_Label", ResourceType = typeof(Resources.Template), Order = 2)]
        [TableColumnDefinition(Sort = true, Order = 2)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Crud_Demo_Salary_Label", ResourceType = typeof(Resources.Template), Order = 4)]
        public double Salary { get; set; }

        [DataType(DataType.Date)]
        [DateFormat("yyyy-mm-dd")]
        [Display(Name = "Crud_Demo_StartDate_Label", ResourceType = typeof(Resources.Template), Order = 7)]
        public DateTime StartDate { get; set; } = DateTime.Now;
    }
}
