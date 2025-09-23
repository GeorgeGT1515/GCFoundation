using GCFoundation.Components.Models.FormBuilder;
using GCFoundation.Web.Models.Components;

namespace GCFoundation.Web.Models
{
    /// <summary>
    /// ViewModel for testing form builder with multiple input types.
    /// </summary>
    public class FormBuilderTestViewModel : ComponentViewModel
    {
        /// <summary>
        /// Instance of a FormViewModel object built using Json.
        /// </summary>
        public FormViewModel? SampleFormBuilder { get; set; }
    }
}