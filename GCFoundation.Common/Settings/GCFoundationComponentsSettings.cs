using GCFoundation.Common.Utilities;
using GCFoundation.Common.Models;
using System.Collections.ObjectModel;

namespace GCFoundation.Common.Settings
{
    /// <summary>
    /// Represents configuration settings for frontend dependencies and application metadata.
    /// Provides centralized access to CDN URIs for GC Design System, Font Awesome,
    /// and multilingual application information.
    /// </summary>
    public class GCFoundationComponentsSettings
    {
        /// <summary>
        /// Gets or sets the version of Font Awesome being used.
        /// </summary>
        public string FontAwesomeVersion { get; set; } = "6.4.2";

        /// <summary>
        /// Gets or sets the version of the GC Design System CSS Shortcuts being used.
        /// </summary>
        public string GCDSCssShortcutsVersion { get; set; } = "1.0.1";

        /// <summary>
        /// Gets or sets the version of the GC Design System being used.
        /// </summary>
        public string GCDSVersion { get; set; } = "0.43.1";


        /// <summary>
        /// Gets the URI for the Font Awesome CSS from the CDN.
        /// </summary>
        public Uri FontAwesomeCDN
        {
            get
            {
                return new Uri($"https://cdnjs.cloudflare.com/ajax/libs/font-awesome/{FontAwesomeVersion}/css/all.min.css");
            }
        }

        /// <summary>
        /// Gets the URI for the GC Design System CSS from the CDN.
        /// </summary>
        public Uri GCDSCssCDN
        {
            get
            {
                return new Uri($"https://cdn.design-system.alpha.canada.ca/@cdssnc/gcds-components@{GCDSVersion}/dist/gcds/gcds.css");
            }
        }

        /// <summary>
        /// Gets the URI for the GC Design System - for CSS Shortcuts - from the CDN.
        /// </summary>
        public Uri GCDSCssShortcutsCDN
        {
            get
            {
                return new Uri($"https://cdn.design-system.alpha.canada.ca/@gcds-core/css-shortcuts@{GCDSCssShortcutsVersion}/dist/gcds-css-shortcuts.min.css");
            }
        }

        /// <summary>
        /// Gets the URI for the GC Design System JavaScript module from the CDN.
        /// </summary>
        public Uri GCDSJavaScriptCDN
        {
            get
            {
                return new Uri($"https://cdn.design-system.alpha.canada.ca/@cdssnc/gcds-components@{GCDSVersion}/dist/gcds/gcds.esm.js");
            }
        }


        /// <summary>
        /// Gets or sets the Adobe Analytics CDN URL for the script (minified).
        /// Example: //assets.adobedtm.com/be5dfd287373/0127575cd23a/launch-f7c3e6060667.min.js
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Configuration property bound from appsettings.json")]
        public string AdobeAnalyticsCdnJsUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets the application name based on the current language context.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return LanguageUtility.IsEnglish() ? ApplicationNameEn : ApplicationNameFr;
            }
        }

        /// <summary>
        /// Gets or sets the English name of the application.
        /// </summary>
        public string ApplicationNameEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the French name of the application.
        /// </summary>
        public string ApplicationNameFr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the version number of the application.
        /// </summary>
        public string ApplicationVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the support link (or mailto) for English users.
        /// </summary>
        public string SupportLinkEn { get; set; } = default!;

        /// <summary>
        /// Gets or sets the support link (or mailto) for French users.
        /// </summary>
        public string SupportLinkFr { get; set; } = default!;

        /// <summary>
        /// Gets or sets the virtual directory name when the app is hosted under a path base.
        /// Example: "/myapp" or "myapp". Leave empty for root.
        /// </summary>
        public string VirtualDirectoryName { get; set; } = string.Empty;


        /// <summary>
        /// Gets the list of additional CSS files to include globally.
        /// These can be local paths or CDN URLs.
        /// </summary>
        public Collection<string> GlobalCssFiles { get; } = new Collection<string>();

        /// <summary>
        /// Gets the list of additional JavaScript files to include globally.
        /// These can be local paths or CDN URLs.
        /// </summary>
        public Collection<string> GlobalJavaScriptFiles { get; } = new Collection<string>();

        /// <summary>
        /// Gets the list of additional meta tags to include in the head section.
        /// Configure with name/content, property/content, http-equiv/content, or charset.
        /// </summary>
        public Collection<MetaTag> GlobalMetaTags { get; } = new Collection<MetaTag>();

        /// <summary>
        /// Gets the list of additional link tags to include in the head section.
        /// Each item should be a complete link tag (e.g., "&lt;link rel=&quot;preconnect&quot; href=&quot;...&quot; /&gt;").
        /// </summary>
        public Collection<string> GlobalLinkTags { get; } = new Collection<string>();


        /// <summary>
        /// Determines whether to include the Adobe Analytics' script.
        /// Set to false to disable automatic reporting to Adobe Analytics.
        /// </summary>
        public bool IncludeAdobeAnalytics { get; set; }

        /// <summary>
        /// Gets or sets whether to include the default foundation CSS files.
        /// Set to false to disable automatic inclusion of foundation.min.css and other default styles.
        /// </summary>
        public bool IncludeDefaultCss { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to include the default foundation JavaScript files.
        /// Set to false to disable automatic inclusion of foundation.min.js and other default scripts.
        /// </summary>
        public bool IncludeDefaultJavaScript { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to include the default GCDS CDN resources.
        /// Set to false to disable automatic inclusion of GCDS CSS and JavaScript from CDN.
        /// </summary>
        public bool IncludeGCDSResources { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to include Font Awesome CDN resources.
        /// Set to false to disable automatic inclusion of Font Awesome CSS from CDN.
        /// </summary>
        public bool IncludeFontAwesome { get; set; } = true;


        // Grid.js configuration
        /// <summary>
        /// Gets or sets whether to include Grid.js resources.
        /// </summary>
        public bool IncludeGridJs { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to load Grid.js from CDN instead of local assets.
        /// </summary>
        public bool UseGridJsCdn { get; set; }

        /// <summary>
        /// Gets or sets the Grid.js CDN URL for the production bundle (minified).
        /// Example: https://unpkg.com/gridjs/dist/gridjs.production.min.js
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Configuration property bound from appsettings.json")]
        public string GridJsCdnJsUrl { get; set; } = "https://unpkg.com/gridjs/dist/gridjs.production.min.js";

        /// <summary>
        /// Gets or sets the Grid.js CDN CSS URL for the default theme (minified).
        /// Example: https://unpkg.com/gridjs/dist/theme/mermaid.min.css
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Configuration property bound from appsettings.json")]
        public string GridJsCdnCssUrl { get; set; } = "https://unpkg.com/gridjs/dist/theme/mermaid.min.css";
    }
}
