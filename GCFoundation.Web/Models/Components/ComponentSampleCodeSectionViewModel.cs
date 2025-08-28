namespace GCFoundation.Web.Models.Components
{
    /// <summary>
    /// ViewModel representing a sample code section of a component.
    /// </summary>
    public class ComponentSampleCodeSectionViewModel
    {
        /// <summary>
        /// Introductory text for the sample code.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Html identifier attribute for the section.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Name of the partial view that contains the sample code.
        /// </summary>
        public string PartialViewName { get; set; } = string.Empty;

        /// <summary>
        /// Title of the section.
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}