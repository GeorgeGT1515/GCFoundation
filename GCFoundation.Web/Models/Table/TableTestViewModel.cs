using GCFoundation.Components.Models.TableBuilder;

namespace GCFoundation.Web.Models.Table
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
        public ICollection<TableRowBasicTestViewModel> BasicRows { get; set; } = new List<TableRowBasicTestViewModel>();

        public ICollection<TableRowEmailTestViewModel> EmailRows { get; set; } = new List<TableRowEmailTestViewModel>();

        public ICollection<TableRowLinkTestViewModel> LinkRows { get; set; } = new List<TableRowLinkTestViewModel>();

        public ICollection<TableRowButtonLinkTestViewModel> ButtonLinkRows { get; set; } = new List<TableRowButtonLinkTestViewModel>();

        /// <summary>
        /// The column definitions describing how each field in <see cref="Rows"/> should be displayed.
        /// </summary>
        public ICollection<ColumnDefinition> Cols { get; set; } = new List<ColumnDefinition>();
    }
}