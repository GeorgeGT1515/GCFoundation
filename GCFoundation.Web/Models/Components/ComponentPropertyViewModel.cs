namespace GCFoundation.Web.Models.Components
{
    /// <summary>
    /// ViewModel representing a property of a component.
    /// </summary>
    public class ComponentPropertyViewModel
    {
        /// <summary>
        /// Type of data expected for the property.
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// Default value for the property.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Description of the purpose of the property.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Name of the property.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}