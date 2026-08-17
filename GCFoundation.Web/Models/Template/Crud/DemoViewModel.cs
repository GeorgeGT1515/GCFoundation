namespace GCFoundation.Web.Models.Template.Crud
{
    public class DemoViewModel
    {
        public IList<EmployeeViewModel> EmployeeModels { get; set; }

        public void DeleteEmployee(string id)
        {
            EmployeeModels.Remove(EmployeeModels.FirstOrDefault(e => e.Id == id));
        }
    }
}
