using GCFoundation.Components.Enums;

namespace GCFoundation.Components.Models.TableBuilder
{
    /// <summary>
    /// Represents the definition of a single column in a table component, describing how its data
    /// should be bound, displayed, and behave (alignment, sorting, row headers, and custom content slots).
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>
        /// The unique id that connects the column to the correct data. The value must match the key
        /// used for that data in your data object.
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// The column heading that appears at the top of the column.
        /// </summary>
        public string Header { get; set; } = string.Empty;

        /// <summary>
        /// Set to <c>true</c> if you want to mark each cell in the column as a row header. Row headers
        /// label what each row is about.
        /// </summary>
        public bool? RowHeader { get; set; }

        /// <summary>
        /// Set to <c>true</c> to flag that the cell will render custom content. To see how each
        /// framework handles this, go to
        /// <see href="https://design-system.canada.ca/en/components/table/code/#framework-specific-slots-for-custom-content">
        /// Framework-specific slots for custom content</see>.
        /// </summary>
        public bool Slotted { get; set; }

        /// <summary>
        /// The type of custom content rendered in the slot when <see cref="Slotted"/> is <c>true</c>.
        /// </summary>
        public SlotType? SlotType { get; set; }

        /// <summary>
        /// Set to <c>true</c> to allow people to sort the table by that column.
        /// </summary>
        public bool? Sort { get; set; }

        /// <summary>
        /// Sets a default sort order for the column when the page loads. Set to <see cref="Enums.SortDirection.Ascending"/>
        /// for ascending order or <see cref="Enums.SortDirection.Descending"/> for descending order.
        /// </summary>
        public SortDirection? SortDirection { get; set; }

        /// <summary>
        /// Controls how the content inside the column cells is positioned horizontally. Set to
        /// <c>start</c> to align content to the left, <c>center</c> to align it to the middle, or
        /// <c>end</c> to align it to the right.
        /// </summary>
        public CellAlignment? Alignment { get; set; }
    }
}