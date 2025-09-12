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
        /// The value of the meta tag 'content' attribute.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Clones a 'meta' tag into a new MetaTag object.
        /// </summary>
        public MetaTag Clone() {
            return new MetaTag
            {
                Name = Name,
                Property = Property,
                HttpEquiv = HttpEquiv,
                Charset = Charset,
                Content = Content
            };
        }

        /// <summary>
        /// Renders a 'meta' tag based on the current attributes of the object.
        /// </summary>
        public string? Render() {
            if (!string.IsNullOrWhiteSpace(Charset))
                return $"<meta charset=\"{System.Net.WebUtility.HtmlEncode(Charset)}\">";

            var attributes = new List<string>();
            if (!string.IsNullOrWhiteSpace(Name))
                attributes.Add($"name=\"{System.Net.WebUtility.HtmlEncode(Name)}\"");
            if (!string.IsNullOrWhiteSpace(Property))
                attributes.Add($"property=\"{System.Net.WebUtility.HtmlEncode(Property)}\"");
            if (!string.IsNullOrWhiteSpace(HttpEquiv))
                attributes.Add($"http-equiv=\"{System.Net.WebUtility.HtmlEncode(HttpEquiv)}\"");
            if (!string.IsNullOrWhiteSpace(Content))
                attributes.Add($"content=\"{System.Net.WebUtility.HtmlEncode(Content)}\"");
            return $"<meta {string.Join(" ", attributes)}>";
        }
    }
}