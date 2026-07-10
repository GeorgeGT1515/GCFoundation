using GCFoundation.Components.Enums;

namespace GCFoundation.Web.Models
{
    public class TableColumnTestViewModel
    {
        public string Field { get; set; }
        public string Header { get; set; }
        public bool RowHeader { get; set; }
        public bool Slotted { get; set; } 
        public SlotType? SlotType { get; set; }
        public string? SlotHrefTemplate { get; set; }
        public string? SlotDisplayField { get; set; }
        public string? SlotActionName { get; set; } // e.g. "edit", "delete", "approve"
        public string? SlotButtonLabel { get; set; }
        public 
    }
}