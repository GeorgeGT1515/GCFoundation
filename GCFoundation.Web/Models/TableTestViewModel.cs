using GCFoundation.Components.Models.TableBuilder;

namespace GCFoundation.Web.Models
{
    public class TableTestViewModel
    {
        public List<TableRowTestViewModel> Rows = new List<TableRowTestViewModel>();
        public List<ColumnDefinition> Cols = new List<ColumnDefinition>();
    }
}
