using GCFoundation.Components.Enums;

namespace GCFoundation.Components.Attributes.Table
{
    /// <summary>
    /// Marks a property as a column definition for a table component. When applied to a property in a model class, it indicates that the property should be treated as a column in the table, and its values will be used to define the table column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class TableColumnDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Use <see cref="Alignment"/> to control how the content inside the column cells is positioned horizontally.
        /// </summary>
        public CellAlignment Alignment { get; set; } = CellAlignment.start;

        /// <summary>
        /// Set <see cref="IsHidden"/> to <c>true</c> if you want to hide the column.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Set <see cref="RowHeader"/> to <c>true</c> if you want to mark each cell in the column as a row header. Row headers label what each row is about.
        /// </summary>
        public bool RowHeader { get; set; }

        /// <summary>
        /// Set <see cref="Slotted"/> to <c>true</c> to flag that the cell will render custom content. To see how each framework handles this, go to <see href="https://design-system.canada.ca/en/components/table/code/#framework-specific-slots-for-custom-content">Framework-specific slots for custom content</see>.
        /// </summary>
        public bool Slotted { get; set; }

        /// <summary>
        /// Set <see cref="Sort"/> to <c>true</c> to allow people to sort the table by that column.
        /// </summary>
        public bool Sort { get; set; }

        /// <summary>
        /// Use <see cref="SortDirection"/> to set a default sort order for the column when the page loads.
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.none;
    }
}