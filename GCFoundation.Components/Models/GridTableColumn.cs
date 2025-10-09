namespace GCFoundation.Components.Models
{
    /// <summary>
    /// Column descriptor for FDCP Grid Table TagHelper.
    /// </summary>
    public class GridTableColumn
    {
        public string Field { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public bool Sortable { get; set; } = true;
        public bool Filter { get; set; } = false;
    }
}


