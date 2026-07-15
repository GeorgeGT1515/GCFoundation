using GCFoundation.Components.Models.TableBuilder;

namespace GCFoundation.Web.Models
{
    /// <summary>
    /// Sample view model used to demo the table component, providing test row data and column
    /// definitions to render.
    /// </summary>
    public class TableTestViewModel
    {
        /// <summary>
        /// The rows of test data to display in the table.
        /// </summary>
        public ICollection<TableRowTestViewModel> Rows { get; set; }  = new List<TableRowTestViewModel>();

        /// <summary>
        /// The column definitions describing how each field in <see cref="Rows"/> should be displayed.
        /// </summary>
        public ICollection<ColumnDefinition> Cols { get; set; } = new List<ColumnDefinition>();
    }
}