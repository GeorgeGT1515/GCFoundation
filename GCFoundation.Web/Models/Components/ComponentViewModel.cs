using GCFoundation.Components.Models;

namespace GCFoundation.Web.Models.Components
{
    /// <summary>
    /// ViewModel representing a component.
    /// </summary>
    public class ComponentViewModel : BaseViewModel
    {
        /// <summary>
        /// Name of the component.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// List of notes regarding the component.
        /// </summary>
        public IEnumerable<string> Notes { get; set; } = new List<string>();

        /// <summary>
        /// Introduction paragraph to present the component and it's purpose or recommended use.
        /// </summary>
        public string Overview { get; set; } = string.Empty;

        /// <summary>
        /// List of properties that can be used with the component.
        /// </summary>
        public IEnumerable<ComponentPropertyViewModel> Properties { get; set; } = new List<ComponentPropertyViewModel>();

        /// <summary>
        /// List of sample code sections for the component.
        /// </summary>
        public IEnumerable<ComponentSampleCodeSectionViewModel> SampleCodeSections { get; set; } = new List<ComponentSampleCodeSectionViewModel>();

        /// <summary>
        /// Should the view display a side navigation or not?
        /// </summary>
        public bool ShowSideNavigation { get; set; } = true;

        private SideNavigationViewModel? sideNavigation;
        /// <summary>
        /// (Optional) Model representing the list of links to the sections of the page for the side navigation.
        /// </summary>
        public SideNavigationViewModel? SideNavigation {
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
        /// Html tag (to be used by developers) that will render the component.
        /// </summary>
        public string Tag { get; set; } = string.Empty;

        /// <summary>
        /// Builds default side navigation based on the current properties of the ViewModel (i.e. include links to the Overview, SampleCodeSections, Properties and Notes).
        /// </summary>
        /// <returns></returns>
        private SideNavigationViewModel DefaultSideNavigation()
        {
            var items = new List<NavItem>();

            if (!string.IsNullOrEmpty(Overview))
                items.Add(new NavLink() { Href = Resources.Components.Overview_Anchor, Label = Resources.Components.Overview });
            if (SampleCodeSections != null && SampleCodeSections.Any())
                items.AddRange(SampleCodeSections.Select(s => new NavLink() { Href = s.Id, Label = s.Title }).ToList());
            if (Notes != null && Notes.Any())
                items.Add(new NavLink() { Href = Resources.Components.Notes_Anchor, Label = Resources.Components.Notes });

            return new SideNavigationViewModel() { Items = items };
        }
    }
}