namespace GCFoundation.Web.Models
{
    /// <summary>
    /// Represents a group of navigation links.
    /// </summary>
    public class NavGroup : NavItem
    {
        /// <summary>
        /// List of navigation items (i.e. NavGroup,.NavLink) contained within the group.
        /// </summary>
        public IEnumerable<NavItem> Items { get; set; } = new List<NavLink>();
    }
}