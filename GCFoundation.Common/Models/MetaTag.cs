using GCFoundation.Common.Utilities;

namespace GCFoundation.Common.Models
{
    /// <summary>
    /// Represents a generic HTML meta tag. Supports either name/content or property/content pairs,
    /// as well as http-equiv and charset use-cases.
    /// </summary>
    public sealed class MetaTag
    {
        /// <summary>
        /// The meta tag 'name' attribute (e.g., description, robots). Mutually exclusive with Property.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The meta tag 'property' attribute (e.g., og:title, twitter:card). Mutually exclusive with Name.
        /// </summary>
        public string? Property { get; set; }

        /// <summary>
        /// The meta tag 'http-equiv' attribute (e.g., X-UA-Compatible, refresh). Rarely used with Name/Property.
        /// </summary>
        public string? HttpEquiv { get; set; }

        /// <summary>
        /// The meta tag 'charset' attribute (e.g., UTF-8). Exclusive of other attributes when used.
        /// </summary>
        public string? Charset { get; set; }

        /// <summary>
        /// The value of the meta tag 'content' attribute, or the language-neutral fallback when
        /// <see cref="ContentEn"/> / <see cref="ContentFr"/> are used.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// English <c>content</c> when the UI culture is English. Falls back to <see cref="Content"/> when unset.
        /// </summary>
        public string? ContentEn { get; set; }

        /// <summary>
        /// French <c>content</c> when the UI culture is French. Falls back to <see cref="Content"/> when unset.
        /// </summary>
        public string? ContentFr { get; set; }

        /// <summary>
        /// Clones a 'meta' tag into a new MetaTag object.
        /// </summary>
        public MetaTag Clone()
        {
            return new MetaTag
            {
                Name = Name,
                Property = Property,
                HttpEquiv = HttpEquiv,
                Charset = Charset,
                Content = Content,
                ContentEn = ContentEn,
                ContentFr = ContentFr
            };
        }

        /// <summary>
        /// Resolves the <c>content</c> value for rendering: French and English use locale-specific properties
        /// when set, otherwise <see cref="Content"/>; other cultures use <see cref="Content"/>, then English, then French.
        /// </summary>
        public string? GetLocalizedContent()
        {
            if (LanguageUtility.IsFrench())
            {
                if (!string.IsNullOrWhiteSpace(ContentFr))
                    return ContentFr;
                return Content;
            }
            else if (LanguageUtility.IsEnglish())
            {
                if (!string.IsNullOrWhiteSpace(ContentEn))
                    return ContentEn;
                return Content;
            }

            if (!string.IsNullOrWhiteSpace(Content))
                return Content;
            if (!string.IsNullOrWhiteSpace(ContentEn))
                return ContentEn;
            if (!string.IsNullOrWhiteSpace(ContentFr))
                return ContentFr;
            return null;
        }

        /// <summary>
        /// Renders a 'meta' tag based on the current attributes of the object.
        /// </summary>
        public string? Render()
        {
            if (!string.IsNullOrWhiteSpace(Charset))
                return $"<meta charset=\"{System.Net.WebUtility.HtmlEncode(Charset)}\">";

            var attributes = new List<string>();
            if (!string.IsNullOrWhiteSpace(Name))
                attributes.Add($"name=\"{System.Net.WebUtility.HtmlEncode(Name)}\"");
            if (!string.IsNullOrWhiteSpace(Property))
                attributes.Add($"property=\"{System.Net.WebUtility.HtmlEncode(Property)}\"");
            if (!string.IsNullOrWhiteSpace(HttpEquiv))
                attributes.Add($"http-equiv=\"{System.Net.WebUtility.HtmlEncode(HttpEquiv)}\"");
            var content = GetLocalizedContent();
            if (!string.IsNullOrWhiteSpace(content))
                attributes.Add($"content=\"{System.Net.WebUtility.HtmlEncode(content)}\"");
            return $"<meta {string.Join(" ", attributes)}>";
        }
    }
}