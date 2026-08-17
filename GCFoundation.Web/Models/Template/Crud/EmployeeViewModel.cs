using GCFoundation.Components.Attributes.Table;

namespace GCFoundation.Web.Models.Template.Crud
{
    public class EmployeeViewModel
    {
        [TableColumnDefinition(Name = "Crud_Table_EmployeeId", ResourceType = typeof(Resources.Template), RowHeader = true, Sort = true)]
        public string Id { get; set; } = string.Empty;
        [TableColumnDefinition(Name = "Crud_Table_EmployeeName", ResourceType = typeof(Resources.Template), Sort = true)]
        public string Name { get; set; } = string.Empty;
        [TableColumnDefinition(Name = "Crud_Table_EmployeeLevel", ResourceType = typeof(Resources.Template), Sort = true)]
        public string Level { get; set; } = string.Empty;

        public double Salary { get; set; }
        public string Department { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;

        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string Address { get; set; } = string.Empty;

        [TableColumnDefinition(Slotted = true)]
        public string Actions { get; set; } = string.Empty;
    }
}
