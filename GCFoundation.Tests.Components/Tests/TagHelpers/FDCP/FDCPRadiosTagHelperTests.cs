using GCFoundation.Components.TagHelpers.FDCP;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.FDCP
{
    public class FDCPRadiosTagHelperTests
    {
        private readonly FDCPRadiosTagHelper _tagHelper;
        private readonly TagHelperContext _context;
        private readonly TagHelperOutput _output;

        public FDCPRadiosTagHelperTests()
        {
            _tagHelper = new FDCPRadiosTagHelper();

            _context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-id");

            _output = new TagHelperOutput("fdcp-radios",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            var viewContext = new ViewContext();
            _tagHelper.ViewContext = viewContext;
        }

        [Fact]
        public void Process_SetsExpectedAttributes_AndOptions()
        {
            var selectedValue = "2";
            SetupModelExpression("NonRequiredProperty", new TestModel { NonRequiredProperty = selectedValue });

            _tagHelper.Items = new List<SelectListItem>()
            {
                new() { Text = "Option 1", Value = "1" },
                new() { Text = "Option 2", Value = "2" }
            };
            _tagHelper.IsRequired = true;

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-radios", _output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, _output.TagMode);
            Assert.Equal("NonRequiredProperty", _output.Attributes["name"].Value);
            Assert.True(_output.Attributes.ContainsName("options"));
            Assert.True(_output.Attributes.ContainsName("required"));
            Assert.Equal(string.Empty, _output.Content.GetContent());

            // Check options JSON
            var optionsAttribute = _output.Attributes.FirstOrDefault(a => a.Name == "options");
            Assert.NotNull(optionsAttribute);

            var options = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                optionsAttribute.Value!.ToString()!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(options);
            Assert.Equal(2, options.Count);

            // Verify first option
            Assert.Equal("NonRequiredProperty_1", options[0]["id"].ToString());
            Assert.Equal("Option 1", options[0]["label"].ToString());
            Assert.Equal("1", options[0]["value"].ToString());
            Assert.False(GetChecked(options[0]));

            // Verify second option
            Assert.Equal("NonRequiredProperty_2", options[1]["id"].ToString());
            Assert.Equal("Option 2", options[1]["label"].ToString());
            Assert.Equal("2", options[1]["value"].ToString());
            Assert.True(GetChecked(options[1]));
        }

        [Fact]
        public void Process_WithoutRequired_SetsNoRequiredAttribute()
        {
            // Arrange
            SetupModelExpression("NonRequiredProperty");
            _tagHelper.Items = new List<SelectListItem>
            {
                new() { Text = "Option 1", Value = "1" }
            };
            _tagHelper.IsRequired = false;

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.False(_output.Attributes.ContainsName("required"));
        }

        [Fact]
        public void Process_WithRequiredAttribute_SetsRequiredAttribute()
        {
            // Arrange
            SetupModelExpression("NonRequiredProperty");
            _tagHelper.Items = new List<SelectListItem>
            {
                new() { Text = "Option 1", Value = "1" }
            };
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
            _tagHelper.Items = new List<SelectListItem>
            {
                new() { Text = "Option 1", Value = "1" }
            };

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
            _tagHelper.Items = new List<SelectListItem>
            {
                new() { Text = "Option 1", Value = "1" }
            };
            _tagHelper.IsRequired = false;

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.False(_output.Attributes.ContainsName("required"));
        }

        [Fact]
        public void Process_NullFor_ThrowsInvalidOperationException()
        {
            // Arrange
            _tagHelper.Items = new List<SelectListItem>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _tagHelper.For = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => _tagHelper.Process(_context, _output));
            Assert.Contains("For is NULL", ex.Message);
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

        [Fact]
        /// <summary>
        /// Test if an int is passed in, if it is properly converted to a string
        /// </summary>
        public void Process_WithRadioAsInt()
        {
            // Arrange
            SetupModelExpression("NonRequiredProperty");
            _tagHelper.Items = new List<SelectListItem>
            {
                new() { Text = "Option 1", Value = "1" }
            };
            _tagHelper.IsRequired = true;

            // Act
            _tagHelper.Process(_context, _output);

            // Assert
            Assert.True(_output.Attributes.ContainsName("required"));
            //Assert.Equal("string", _output.Attributes["type"].Value);
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




        private class TestModel
        {
            [Display(Name = "Non-Required Property")]
            public string NonRequiredProperty { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Required Property")]
            public string RequiredProperty { get; set; } = string.Empty;
        }
    }
}