namespace GCFoundation.Components.Models.TableGridJs
{
    /// <summary>
    /// Represents the definition of a column of Grid.js table.
    /// </summary>
    public class TableGridJsColumn
    {
        /// <summary>
        /// (Optional) Identifier of the column.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Determines whether or not the column will be displayed to the end-user.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Determines whether or not the column should be defined as a header of its row (i.e. &lt;th scope="row"&gt;).
        /// </summary>
        public bool IsRowHeader { get; set; }

        /// <summary>
        /// Determines whether or not the column will be sortable.
        /// </summary>
        public bool IsSortable { get; set; } = true;

        /// <summary>
        /// Name of the column.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Width of the column.
        /// </summary>
        public string? Width { get; set; }
    }
}