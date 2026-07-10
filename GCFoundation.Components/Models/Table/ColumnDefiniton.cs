using GCFoundation.Components.Enums;

namespace GCFoundation.Components.Models.TableBuilder
{
    public class ColumnDefiniton
    {
        public string Field { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public bool? RowHeader { get; set; }
        public bool Slotted { get; set; } 
        public SlotType? SlotType { get; set; }
        public bool? Sort { get; set; }
        public SortDirection? SortDirection { get; set; }
        public CellAlignment? Alignment { get; set; }
    }
}
