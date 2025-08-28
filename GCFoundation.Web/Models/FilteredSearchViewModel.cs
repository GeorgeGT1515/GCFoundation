using GCFoundation.Components.Models;
using GCFoundation.Web.Models.Components;

namespace GCFoundation.Web.Models
{
    /// <summary>
    /// ViewModel for testing the filtered search component.
    /// </summary>
    public class FilteredSearchViewModel : ComponentViewModel
    {
        /// <summary>
        /// List of filter categories to be displayed by the Filtered Search component.
        /// </summary>
        public IEnumerable<SearchFilterCategory> SearchFilterCategories { get; set; } = new List<SearchFilterCategory>();
    }
}