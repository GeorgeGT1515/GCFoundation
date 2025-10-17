using Microsoft.AspNetCore.Mvc;

namespace GCFoundation.Components.Models.TableGridJs
{
    /// <summary>
    /// Represents a query for a Table/Grid.js component.
    /// </summary>
    public class TableGridJsQuery
    {
        /// <summary>
        /// Current page.
        /// </summary>
        [FromQuery(Name = "page")] public int Page { get; set; } = 1;
        /// <summary>
        /// Size of the pages.
        /// </summary>
        [FromQuery(Name = "pageSize")] public int PageSize { get; set; } = 25;
        /// <summary>
        /// Id of the field/column that the query is currently sorted on.
        /// </summary>
        [FromQuery(Name = "sortBy")] public string? SortBy { get; set; }
        /// <summary>
        /// Direction the sorting.
        /// </summary>
        [FromQuery(Name = "sortDir")] public string? SortDir { get; set; }
        /// <summary>
        /// Text of the query/filter.
        /// </summary>
        [FromQuery(Name = "q")] public string? Q { get; set; }
    }
}