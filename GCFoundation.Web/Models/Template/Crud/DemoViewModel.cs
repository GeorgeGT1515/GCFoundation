using GCFoundation.Components.Models;

namespace GCFoundation.Web.Models.Template.Crud
{
    public class DemoViewModel : BaseViewModel
    {
        public IList<EmployeeViewModel> EmployeeModels { get; set; }

        public void DeleteEmployee(string id)
        {
            EmployeeModels.Remove(EmployeeModels.FirstOrDefault(e => e.Id == id));
        }
    }
}
