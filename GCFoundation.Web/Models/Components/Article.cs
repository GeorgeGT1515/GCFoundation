namespace GCFoundation.Web.Models.Components
{
    /// <summary>
    /// Represents a sample article object.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Title of the article.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Name of the author of the article.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Summary of the article.
        /// </summary>
        public string Summary { get; set; } = string.Empty;
    }
}