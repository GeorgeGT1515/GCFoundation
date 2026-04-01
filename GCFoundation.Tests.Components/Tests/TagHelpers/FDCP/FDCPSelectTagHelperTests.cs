using GCFoundation.Components.TagHelpers.FDCP;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.FDCP
{
    public class FDCPSelectTagHelperTests
    {
        private readonly TagHelperContext _context;
        private readonly TagHelperOutput _output;

        public FDCPSelectTagHelperTests()
        {
            _context = new TagHelperContext(
               new TagHelperAttributeList(),
               new Dictionary<object, object>(),
               "test"
            );
            _output = new TagHelperOutput(
                "fdcp-select",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
            );
        }

        [Fact]
        public void Process_ShouldGenerateSelectElement()
        {
            // Arrange
            var tagHelper = new FDCPSelectTagHelper
            {
                For = MockModelExpression("SelectedCountry", "US"),
                Items = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CA", Text = "Canada" },
                    new SelectListItem { Value = "US", Text = "United States" }
                }
            };

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-select", _output.TagName);
            Assert.Contains("United States", _output.Content.GetContent());
            Assert.Contains("Canada", _output.Content.GetContent());
            Assert.Contains("selected", _output.Content.GetContent()); // Ensures correct value is selected
        }

        [Fact]
        public void Process_WithDefaultValue_RendersCorrectly()
        {
            // Arrange
            var tagHelper = new FDCPSelectTagHelper
            {
                DefaultValue = "Select an option / Sélectionner une option",
                For = MockModelExpression("SelectedCountry", "US"),
                Items = new List<SelectListItem>
                {
                    new SelectListItem { Value = "CA", Text = "Canada" },
                    new SelectListItem { Value = "US", Text = "United States" }
                }
            };

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-select", _output.TagName);
            Assert.True(_output.Attributes.ContainsName("default-value"));
            Assert.Equal("Select an option / Sélectionner une option", _output.Attributes["default-value"].Value);
        }

        private ModelExpression MockModelExpression(string name, string value)
        {
            var metadataProvider = new EmptyModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForType(typeof(string));
            var modelExpression = new ModelExpression(name, new ModelExplorer(metadataProvider, metadata, value));
            return modelExpression;
        }
    }
}
