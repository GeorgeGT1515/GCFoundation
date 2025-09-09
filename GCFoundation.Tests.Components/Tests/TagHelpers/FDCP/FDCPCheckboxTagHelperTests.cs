using GCFoundation.Components.TagHelpers.FDCP;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.FDCP
{
    public class FDCPCheckboxTagHelperTests
    {
        private readonly FDCPCheckboxTagHelper _tagHelper;
        private readonly TagHelperContext _context;
        private readonly TagHelperOutput _output;

        public FDCPCheckboxTagHelperTests()
        {
            _tagHelper = new FDCPCheckboxTagHelper();

            _context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-id");

            _output = new TagHelperOutput("fdcp-checkbox",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            var viewContext = new ViewContext();
            _tagHelper.ViewContext = viewContext;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Process_GeneratesCorrectOptionsJson(bool isChecked)
        {
            // Arrange
            SetupModelExpression("NonRequiredProperty", new TestModel { NonRequiredProperty = isChecked });

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            var optionsAttribute = Assert.Single(_output.Attributes, a => a.Name == "options");
            Assert.NotNull(optionsAttribute.Value);

            var options = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                optionsAttribute.Value.ToString()!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(options);
            Assert.Single(options);

            var option = options[0];
            Assert.Equal("NonRequiredProperty", option["id"].ToString());
            Assert.Equal("Non-Required Property", option["label"].ToString());
            Assert.Equal("true", option["value"].ToString());
            Assert.Equal(isChecked, GetChecked(option));
        }

        [Fact]
        public void Process_RendersCorrectTagName()
        {
            // Arrange
            SetupModelExpression("NonRequiredProperty");

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-checkboxes", _output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, _output.TagMode);
        }

        [Fact]
        public void Process_WithNullFor_ThrowsInvalidOperationException()
        {
            // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _tagHelper.For = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _tagHelper.Process(_context, _output));
        }

        [Fact]
        public void Process_WithRequiredAttribute_SetsRequiredAttribute()
        {
            // Arrange
            SetupModelExpression("NonRequiredProperty");
            _tagHelper.IsRequired = true;

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.True(_output.Attributes.ContainsName("required"));
        }

        [Fact]
        public void Process_WithRequiredDataAnnotation_WithoutRequiredAttribute_SetsRequiredAttribute()
        {
            // Arrange
            SetupModelExpression("RequiredProperty");

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.True(_output.Attributes.ContainsName("required"));
        }

        [Fact]
        public void Process_WithRequiredDataAnnotation_OverridesWithFalseRequiredAttribute_SetsNoRequiredAttribute()
        {
            // Arrange
            SetupModelExpression("RequiredProperty");
            _tagHelper.IsRequired = false;

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.False(_output.Attributes.ContainsName("required"));
        }

        /// <summary>
        /// Sets up the ModelExpression for the properties of the TestModel.
        /// </summary>
        private void SetupModelExpression(string propertyName, TestModel? model = null)
        {
            var modelType = typeof(TestModel);
            var instance = model ?? new TestModel();

            var metadataProvider = new EmptyModelMetadataProvider();
            var modelExplorer = metadataProvider.GetModelExplorerForType(modelType, instance);

            var propertyExplorer = modelExplorer.GetExplorerForProperty(propertyName);
            var modelExpression = new ModelExpression(propertyName, propertyExplorer);
            _tagHelper.For = modelExpression;
        }

        /// <summary>
        /// Determines if an option is "checked" or not.
        /// </summary>
        private static bool GetChecked(Dictionary<string, object> option)
        {
            if (option["checked"] is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.True) return true;
                if (je.ValueKind == JsonValueKind.False) return false;
                if (je.ValueKind == JsonValueKind.String)
                    return bool.Parse(je.GetString()!);
            }
            if (option["checked"] is bool b)
                return b;
            if (option["checked"] is string s)
                return bool.Parse(s);
            throw new InvalidCastException("Cannot convert checked value to bool.");
        }

        public class TestModel
        {
            [Display(Name = "Non-Required Property")]
            public bool NonRequiredProperty { get; set; } = false;

            [Required]
            [Display(Name = "Required Property")]
            public bool RequiredProperty { get; set; } = false;
        }
    }
}