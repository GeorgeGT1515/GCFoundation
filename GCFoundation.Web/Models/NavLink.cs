namespace GCFoundation.Web.Models
{
    /// <summary>
    /// Represents a navigation link.
    /// </summary>
    public class NavLink : NavItem
    {
        /// <summary>
        /// Href of the link.
        /// </summary>
        public string Href { get; set; } = string.Empty;
    }
}