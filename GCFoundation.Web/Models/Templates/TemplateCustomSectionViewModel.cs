namespace GCFoundation.Web.Models.Templates
{
    /// <summary>
    /// ViewModel representing a custom section of a page template.
    /// </summary>
    public class TemplateCustomSectionViewModel
    {
        /// <summary>
        /// Introductory text for the custom section.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Html identifier attribute for the section.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Name of the partial view that contains the custom section.
        /// </summary>
        public string PartialViewName { get; set; } = string.Empty;

        /// <summary>
        /// Title of the section.
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}