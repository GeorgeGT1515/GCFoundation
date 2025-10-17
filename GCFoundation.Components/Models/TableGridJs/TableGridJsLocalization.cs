using GCFoundation.Components.Enums;
using System.Globalization;

namespace GCFoundation.Components.Models.TableGridJs
{
    /// <summary>
    /// Represents the localization configuration of a Grid.js table.
    /// </summary>
    public class TableGridJsLocalization
    {
        /// <summary>
        /// Localized message to be displayed when data cannot be fetched.
        /// </summary>
        public string? ErrorFetchText { get; set; }

        /// <summary>
        /// Localized message to be displayed while the content is being loaded.
        /// </summary>
        public string? LoadingText { get; set; }

        /// <summary>
        /// Localized message to be displayed when there is no content to be displayed.
        /// </summary>
        public string? NoDataText { get; set; }

        /// <summary>
        /// Localized message to be displayed when there is no search results matching the current criteria.
        /// </summary>
        public string? NoResultsText { get; set; }

        /// <summary>
        /// Localized message to be displayed on the next page button.
        /// </summary>
        public string? PaginationNext { get; set; }

        /// <summary>
        /// Localized message to be displayed on the pagination summary text - more specifically the "of" keyword in "Showing X to Y results of Z results".
        /// </summary>
        public string? PaginationOf { get; set; }

        /// <summary>
        /// Localized message to be displayed on the previous page button.
        /// </summary>
        public string? PaginationPrevious { get; set; }

        /// <summary>
        /// Localized message to be displayed on the pagination summary text - more specifically the "results" keyword in "Showing X to Y results of Z results".
        /// </summary>
        public string? PaginationResults { get; set; }

        /// <summary>
        /// Localized message to be displayed on the pagination summary text - more specifically the "Showing" keyword in "Showing X to Y results of Z results".
        /// </summary>
        public string? PaginationShowing { get; set; }

        /// <summary>
        /// Localized message to be displayed on the pagination summary text - more specifically the "to" keyword in "Showing X to Y results of Z results".
        /// </summary>
        public string? PaginationTo { get; set; }

        /// <summary>
        /// Localized message to be used as the aria-label attribute of the filtering search input field.
        /// </summary>
        public string? SearchAriaLabel { get; set; }

        /// <summary>
        /// Localized message to be used as the placeholder text of the filtering search input field.
        /// </summary>
        public string? SearchPlaceholder { get; set; }

        /// <summary>
        /// Localized message to be used as the aria-label and title attributes of the sorting button - for sorting the column in an ascending order.
        /// </summary>
        public string? SortAscending { get; set; }

        /// <summary>
        /// Localized message to be used as the aria-label and title attributes of the sorting button - for sorting the column in an descending order.
        /// </summary>
        public string? SortDescending { get; set; }

        /// <summary>
        /// Localize the properties of the object based on the <paramref name="language"/> parameter.
        /// </summary>
        /// <param name="language">Language inwhich the properties should be localized.</param>
        public void Localize(Language language)
        {
            // Set culture based on Lang attribute if provided
            CultureInfo culture;
            switch (language)
            {
                case Language.fr:
                    culture = new CultureInfo("fr");
                    break;
                case Language.en:
                default:
                    culture = new CultureInfo("en");
                    break;
            }

            // Get localized strings using strongly-typed resources.
            var rm = Resources.GridTable.ResourceManager;

            ErrorFetchText = rm.GetString("ErrorFetch_Text", culture) ?? "An error happened while fetching the data";
            LoadingText = rm.GetString("Loading_Text", culture) ?? "Loading...";
            NoDataText = rm.GetString("NoData_Text", culture) ?? "No data";
            NoResultsText = rm.GetString("NoResults_Text", culture) ?? "No records found";
            PaginationNext = rm.GetString("Pagination_Next", culture) ?? "Next";
            PaginationOf = rm.GetString("Pagination_Of", culture) ?? "of";
            PaginationPrevious = rm.GetString("Pagination_Previous", culture) ?? "Previous";
            PaginationResults = rm.GetString("Pagination_Results", culture) ?? "results";
            PaginationShowing = rm.GetString("Pagination_Showing", culture) ?? "Showing";
            PaginationTo = rm.GetString("Pagination_To", culture) ?? "to";
            SearchAriaLabel = rm.GetString("Search_AriaLabel", culture) ?? "Search table";
            SearchPlaceholder = rm.GetString("Search_Placeholder", culture) ?? "Search...";
            SortAscending = rm.GetString("Sort_Ascending", culture) ?? "Sort column ascending";
            SortDescending = rm.GetString("Sort_Descending", culture) ?? "Sort column descending";
        }
    }
}