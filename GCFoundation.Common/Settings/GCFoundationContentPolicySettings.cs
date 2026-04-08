namespace GCFoundation.Common.Settings
{
    /// <summary>
    /// Represents the configuration settings used to build a Content Security Policy (CSP).
    /// These settings define which external resources (CDNs, fonts) are allowed by the application.
    /// </summary>
    public class GCFoundationContentPolicySettings
    {
        /// <summary>
        /// Gets or sets the list of origins allowed for network connections (XHR/fetch/WebSocket/EventSource).
        /// These will be added to the 'connect-src' directive in the CSP header.
        /// Example: "https://cdn.design-system.alpha.canada.ca"
        /// </summary>
        public IEnumerable<string> ConnectSrc { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the list of font CDN hosts that are allowed to load fonts.
        /// These will be added to the 'font-src' directive in the CSP header.
        /// Example: "https://fonts.gstatic.com"
        /// </summary>
        public IEnumerable<string> FontSrc { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the list of hosts that are allowed to load frames.
        /// These will be added to the 'frame-src' directive in the CSP header.
        /// </summary>
        public IEnumerable<string> FrameSrc { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the list of hosts that are allowed to load images.
        /// These will be added to the 'img-src' directive in the CSP header.
        /// </summary>
        public IEnumerable<string> ImgSrc { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the list of JavaScript CDN hosts that are allowed to load scripts.
        /// These will be added to the 'script-src' directive in the CSP header.
        /// Example: "https://cdn.jsdelivr.net"
        /// </summary>
        public IEnumerable<string> ScriptSrc { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets the list of CSS CDN hosts that are allowed to load stylesheets.
        /// These will be added to the 'style-src' directive in the CSP header.
        /// Example: "https://fonts.googleapis.com"
        /// </summary>
        public IEnumerable<string> StyleSrc { get; set; } = Enumerable.Empty<string>();
    }
}
