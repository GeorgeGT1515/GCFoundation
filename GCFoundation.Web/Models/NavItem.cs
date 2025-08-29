namespace GCFoundation.Web.Models
{
    /// <summary>
    /// Interface representing a navigation item (e.g. NavGroup, NavLink).
    /// </summary>
    public abstract class NavItem
    {
        /// <summary>
        /// Label of a NavItem.
        /// </summary>
        public string Label { get; set; } = string.Empty;
    }
}