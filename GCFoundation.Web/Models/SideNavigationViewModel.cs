namespace GCFoundation.Web.Models
{
    /// <summary>
    /// ViewModel representing a side navigation.
    /// </summary>
    public class SideNavigationViewModel
    {
        /// <summary>
        /// List of navigation items (i.e. NavGroup,.NavLink) contained within the side navigation.
        /// </summary>
        public IEnumerable<NavItem>? Items { get; set; }
    }
}