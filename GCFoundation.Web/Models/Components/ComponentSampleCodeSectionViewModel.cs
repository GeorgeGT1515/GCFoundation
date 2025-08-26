namespace GCFoundation.Web.Models.Components
{
    /// <summary>
    /// ViewModel representing a sample code section of a component.
    /// </summary>
    public class ComponentSampleCodeSectionViewModel
    {
        /// <summary>
        /// Formatted sample code for the section.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Language of the sample code for the section (for formatting).
        /// </summary>
        public string CodeLanguage { get; set; } = string.Empty;

        /// <summary>
        /// Html identifier attribute for the section.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// HTML content that will be rendered as is to preview the result of the sample code.
        /// </summary>
        public string HtmlContent { get; set; } = string.Empty;

        /// <summary>
        /// Title of the section.
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}