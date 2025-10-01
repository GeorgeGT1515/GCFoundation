namespace GCFoundation.Web.Models.Templates
{
    /// <summary>
    /// ViewModel representing a page template.
    /// </summary>
    public class TemplateViewModel
    {
        /// <summary>
        /// Name of the page template.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of notes/highlights regarding the page template.
        /// </summary>
        public IEnumerable<string> Notes { get; set; } = new List<string>();

        /// <summary>
        /// Introduction paragraph to present the page template and it's purpose or recommended use.
        /// </summary>
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// List of custom sections for the page template.
        /// </summary>
        /// <remarks>
        /// This could be used to present some considerations and/or alternative uses for the page template.
        /// </remarks>
        public IEnumerable<TemplateCustomSectionViewModel> CustomSections { get; set; } = new List<TemplateCustomSectionViewModel>();

        /// <summary>
        /// Short description of the page template. Displayed on the index page - in the list of page templates.
        /// </summary>
        public string ShortDescription { get; set; } = string.Empty;

        /// <summary>
        /// Should the view display a side navigation or not?
        /// </summary>
        public bool ShowSideNavigation { get; set; } = true;

        private SideNavigationViewModel? sideNavigation;
        /// <summary>
        /// (Optional) Model representing the list of links to the sections of the page for the side navigation.
        /// </summary>
        public SideNavigationViewModel? SideNavigation
        {
            get
            {
                if (sideNavigation == null)
                    return DefaultSideNavigation();
                else
                    return sideNavigation;
            }
            set
            {
                sideNavigation = value;
            }
        }

        /// <summary>
        /// Builds default side navigation based on the current properties of the ViewModel (i.e. include links to the Overview, CustomSections and Notes).
        /// </summary>
        /// <returns></returns>
        private SideNavigationViewModel DefaultSideNavigation()
        {
            var items = new List<NavItem>();

            if (!string.IsNullOrEmpty(Overview))
                items.Add(new NavLink() { Href = Resources.Components.Overview_Anchor, Label = Resources.Components.Overview });
            if (CustomSections != null && CustomSections.Any())
                items.AddRange(CustomSections.Select(s => new NavLink() { Href = s.Id, Label = s.Title }).ToList());
            if (Notes != null && Notes.Any())
                items.Add(new NavLink() { Href = Resources.Components.Notes_Anchor, Label = Resources.Components.Notes });

            return new SideNavigationViewModel() { Items = items };
        }
    }
}