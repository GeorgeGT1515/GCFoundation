namespace GCFoundation.Components.Models.TableGridJs
{
    /// <summary>
    /// Represents the definition of a column of Grid.js table.
    /// </summary>
    public class TableGridJsColumn
    {
        /// <summary>
        /// Determines whether or not the column will be displayed to the end-user.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// (Optional) Identifier of the column.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Name of the column.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Determines whether or not the column will be sortable.
        /// </summary>
        public bool Sortable { get; set; } = true;

        /// <summary>
        /// Width of the column.
        /// </summary>
        public string? Width { get; set; }
    }
}