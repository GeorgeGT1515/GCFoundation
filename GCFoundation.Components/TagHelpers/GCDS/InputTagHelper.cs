using GCFoundation.Components.Enums;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GCFoundation.Components.TagHelpers.GCDS
{
    /// <summary>
    /// Represents a custom TagHelper for rendering an input element with a label and other associated attributes.
    /// </summary>
    [HtmlTargetElement("gcds-input", Attributes = "for")]
    public class InputTagHelper : BaseFormComponentTagHelper
    {
        /// <summary>
        /// Gets or sets the ID for the input element. 
        /// If not set, it is auto-derived from the `For.Name` property.
        /// </summary>
        public string? InputId { get; set; }

        /// <summary>
        /// Gets or sets the label for the input element.
        /// If not set, it is auto-derived from the `DisplayName` attribute or `For.Name`.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the autocomplete behavior for the input element.
        /// Defaults to <see cref="AutocompleteType.off"/>.
        /// </summary>
        public AutocompleteType Autocomplete { get; set; } = AutocompleteType.off;

        /// <summary>
        /// Gets or sets whether to hide the label.
        /// </summary>
        public bool HideLabel { get; set; }

        /// <summary>
        /// Gets or sets the validation event trigger. Defaults to "blur" as per GCDS documentation.
        /// </summary>
        public new string ValidateOn { get; set; } = "blur";

        /// <summary>
        /// Gets or sets the size of the input field in characters.
        /// </summary>
        public int? Size { get; set; }

        /// <inheritdoc/>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Auto-derive InputId and Label if not provided
            if (For != null)
            {
                InputId ??= For.Name;
                Label ??= For.Metadata.DisplayName ?? For.Name;

                RetrieveLocalizedProperties();
            }

            // Required GCDS attributes (name, label, input-id)
            AddAttributeIfNotNull(output, "name", For?.Name);
            AddAttributeIfNotNull(output, "input-id", InputId);
            AddAttributeIfNotNull(output, "label", Label);
            
            // Optional attributes
            AddAttributeIfNotNull(output, "hide-label", HideLabel);
            AddAttributeIfNotNull(output, "autocomplete", Autocomplete);
            AddAttributeIfNotNull(output, "size", Size);
            
            // Validation attributes
            AddAttributeIfNotNull(output, "validate-on", ValidateOn);

            base.Process(context, output);
        }

        /// <summary>
        /// Retrieves localized properties such as the label from metadata.
        /// </summary>
        private void RetrieveLocalizedProperties()
        {
            var propertyInfo = For.Metadata.ContainerType?.GetProperty(For.Name);
            if (propertyInfo == null) return;

            // Use GetLocalizedLabel to retrieve the label
            Label = GetLocalizedLabel(propertyInfo);
        }

        /// <summary>
        /// Retrieves the localized label for the input field from its associated <see cref="DisplayAttribute"/>.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/> representing the property of the input field.</param>
        /// <returns>The localized label if available, otherwise the property name.</returns>
        protected static string GetLocalizedLabel(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));
            var displayAttr = property.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.GetName() ?? property.Name;
        }

    }
}
