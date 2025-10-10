using GCFoundation.Components.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GCFoundation.Components.TagHelpers.FDCP
{
    /// <summary>
    /// Renders an accessible Grid.js-powered table container with progressive enhancement.
    /// Outputs a semantic table skeleton for no-JS environments and a data bootstrap for JS.
    /// Server-side sorting, searching, and pagination are enforced.
    /// </summary>
    [HtmlTargetElement("fdcp-table-gridjs")]
    public sealed class FDCPTableGridJsTagHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        /// <summary>
        /// Unique id for the grid container. If not provided, one will be generated.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Required. AJAX endpoint returning the standard envelope: items,total,page,pageSize.
        /// </summary>
        [HtmlAttributeName("ajax-url")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Endpoint path")] 
        public string AjaxUrl { get; set; } = string.Empty;

        /// <summary>
        /// Column definitions: header text, field key, sortable (default true).
        /// </summary>
        public IEnumerable<GridTableColumn>? Columns { get; set; }

        /// <summary>
        /// Page size (default 25). Server-side enforced.
        /// </summary>
        public int PageSize { get; set; } = 25;

        /// <summary>
        /// Enable search (default true). Always server-side.
        /// </summary>
        public bool SearchEnabled { get; set; } = true;

        /// <summary>
        /// Enable sort (default true). Always server-side.
        /// </summary>
        public bool SortEnabled { get; set; } = true;

        /// <summary>
        /// Visible caption text (required for AAA).
        /// </summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Optional summary/description referenced via aria-describedby.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Optional aria-label for the table element.
        /// </summary>
        public string? AriaLabel { get; set; }

        /// <summary>
        /// Localized string when no records found (fallbacks applied client-side as well).
        /// </summary>
        public string? NoDataText { get; set; }

        /// <summary>
        /// Localized loading text.
        /// </summary>
        public string? LoadingText { get; set; }

        /// <summary>
        /// Optional CSS classes to add to the enhanced table element.
        /// </summary>
        public string? Class { get; set; }

        /// <summary>
        /// Language hint ("en" or "fr") for client messages.
        /// </summary>
        public string? Lang { get; set; }

        /// <summary>
        /// Debounce for search in milliseconds (default 300).
        /// </summary>
        public int DebounceMs { get; set; } = 300;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ArgumentNullException.ThrowIfNull(output);
            if (string.IsNullOrWhiteSpace(AjaxUrl))
            {
                throw new InvalidOperationException("ajax-url is required for fdcp-table-gridjs.");
            }
            if (string.IsNullOrWhiteSpace(Caption))
            {
                throw new InvalidOperationException("Caption is required for fdcp-table-gridjs to meet AAA.");
            }

            // Set culture based on Lang attribute if provided
            CultureInfo culture;
            if (!string.IsNullOrWhiteSpace(Lang))
            {
                try
                {
                    culture = new CultureInfo(Lang);
                }
                catch (CultureNotFoundException)
                {
                    // Fall back to current culture if invalid
                    culture = CultureInfo.CurrentUICulture;
                }
            }
            else
            {
                culture = CultureInfo.CurrentUICulture;
            }

            // Get localized strings using strongly-typed resources
            var rm = Resources.GridTable.ResourceManager;
            var searchPlaceholder = rm.GetString("Search_Placeholder", culture) ?? "Search...";
            var searchAriaLabel = rm.GetString("Search_AriaLabel", culture) ?? "Search table";
            var paginationPrevious = rm.GetString("Pagination_Previous", culture) ?? "Previous";
            var paginationNext = rm.GetString("Pagination_Next", culture) ?? "Next";
            var paginationShowing = rm.GetString("Pagination_Showing", culture) ?? "Showing";
            var paginationResults = rm.GetString("Pagination_Results", culture) ?? "results";
            var loadingText = rm.GetString("Loading_Text", culture) ?? "Loading...";
            var noResultsText = rm.GetString("NoResults_Text", culture) ?? "No records found";
            var noDataText = rm.GetString("NoData_Text", culture) ?? "No data";

            var id = string.IsNullOrWhiteSpace(Id) ? $"fdcp-grid-{Guid.NewGuid():N}" : Id!;

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("id", id);
            output.Attributes.SetAttribute("class", "fdcp-grid-container gridjs-container");

            // Build client config payload with localized strings
            var clientConfig = new ClientConfig
            {
                dataUrl = AjaxUrl,
                pageSize = PageSize,
                searchEnabled = SearchEnabled,
                sortEnabled = SortEnabled,
                columns = Columns?.Select(c => new ClientColumn { field = c.Field, header = c.Header, sortable = c.Sortable }).ToArray() ?? Array.Empty<ClientColumn>(),
                tableClass = Class,
                ariaLabel = AriaLabel,
                noDataText = NoDataText ?? noDataText,
                loadingText = LoadingText ?? loadingText,
                lang = culture.TwoLetterISOLanguageName,
                debounceMs = DebounceMs,
                // Add localized UI strings for Grid.js
                searchPlaceholder = searchPlaceholder,
                searchLabel = searchAriaLabel,
                paginationPrevious = paginationPrevious,
                paginationNext = paginationNext,
                paginationShowing = paginationShowing,
                paginationResults = paginationResults,
                noResultsText = noResultsText
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var cfgJson = JsonSerializer.Serialize(clientConfig, jsonOptions);

            // Output markup: live region, controls, semantic table fallback
            var summaryId = !string.IsNullOrWhiteSpace(Summary) ? $"{id}-summary" : null;
            var captionHtml = $"<caption>{System.Net.WebUtility.HtmlEncode(Caption)}</caption>";
            var summaryHtml = summaryId != null ? $"<div id=\"{summaryId}\" class=\"fdcp-sr-only\">{System.Net.WebUtility.HtmlEncode(Summary)}</div>" : string.Empty;

            output.Attributes.SetAttribute("data-fdcp-grid", cfgJson);
            output.Content.AppendHtml($@"
{summaryHtml}
<div class='fdcp-grid-controls'>
  <!-- Grid.js will render its own search and pagination; this container exists for structure/fallback -->
</div>
<noscript>
  <table class='fdcp-table fdcp-table-hover fdcp-table-striped' role='table' aria-describedby='{summaryId}'>
    {captionHtml}
    <thead>
      <tr>
        {RenderHeaders(Columns)}
      </tr>
    </thead>
    <tbody>
      <tr><td>{System.Net.WebUtility.HtmlEncode(NoDataText ?? (Lang == "fr" ? "Aucune donnée" : "No data"))}</td></tr>
    </tbody>
  </table>
</noscript>
            ");
        }

        private static string RenderHeaders(IEnumerable<GridTableColumn>? columns)
        {
            if (columns == null) return string.Empty;
            return string.Join(string.Empty, columns.Select(c => $"<th scope='col'>{System.Net.WebUtility.HtmlEncode(c.Header)}</th>"));
        }

        private sealed class ClientConfig
        {
            public string? dataUrl { get; set; }
            public int pageSize { get; set; }
            public bool searchEnabled { get; set; }
            public bool sortEnabled { get; set; }
            public IEnumerable<ClientColumn>? columns { get; set; }
            public string? tableClass { get; set; }
            public string? ariaLabel { get; set; }
            public string? noDataText { get; set; }
            public string? loadingText { get; set; }
            public string? lang { get; set; }
            public int debounceMs { get; set; }
            // Localized UI strings
            public string? searchPlaceholder { get; set; }
            public string? searchLabel { get; set; }
            public string? paginationPrevious { get; set; }
            public string? paginationNext { get; set; }
            public string? paginationShowing { get; set; }
            public string? paginationResults { get; set; }
            public string? noResultsText { get; set; }
        }

        private sealed class ClientColumn
        {
            public string? field { get; set; }
            public string? header { get; set; }
            public bool? sortable { get; set; }
        }
    }
}


