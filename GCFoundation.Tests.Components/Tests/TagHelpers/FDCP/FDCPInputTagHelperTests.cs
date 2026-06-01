using GCFoundation.Components.Attributes;
using GCFoundation.Components.TagHelpers.FDCP;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using FDCPInputType = GCFoundation.Components.Enums.InputType;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.FDCP
{
    public class FDCPInputTagHelperTests
    {
        private readonly TagHelperContext _context;
        private readonly TagHelperOutput _output;

        public FDCPInputTagHelperTests()
        {
            _context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-id");

            _output = new TagHelperOutput("fdcp-input",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
        }

        private class TestModel
        {
            public string TextProperty { get; set; } = string.Empty;

            [DataType(DataType.EmailAddress)]
            public string EmailProperty { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            public string PasswordProperty { get; set; } = string.Empty;

            [DataType(DataType.PhoneNumber)]
            public string PhoneProperty { get; set; } = string.Empty;

            [DataType(DataType.ImageUrl)]
            public string ImageUrlProperty { get; set; } = string.Empty;

            [DataType(DataType.Date)]
            public DateTime DateProperty { get; set; }

            [DataType(DataType.DateTime)]
            public DateTime DateTimeProperty { get; set; }

            public bool BoolProperty { get; set; }

            [DataType(DataType.MultilineText)]
            public string MultilineProperty { get; set; } = string.Empty;

            public int NumberProperty { get; set; }

            [DataType(DataType.Date)]
            [DateFormat("short")]
            public DateTime DateWithFormatProperty { get; set; }

            [Required]
            public string RequiredTextProperty { get; set; } = string.Empty;
        }

        private FDCPInputTagHelper SetupTagHelper(string propertyName, TestModel? model = null)
        {
            var modelType = typeof(TestModel);
            var modelExplorer = new EmptyModelMetadataProvider()
                .GetModelExplorerForType(modelType, model ?? new TestModel());

            var propertyExplorer = modelExplorer.GetExplorerForProperty(propertyName);
            var modelExpression = new ModelExpression(propertyName, propertyExplorer);

            var tagHelper = new FDCPInputTagHelper()
            {
                For = modelExpression,
                ViewContext = new ViewContext()
            };

            return tagHelper;
        }

        [Fact]
        public void Process_WithNullOutput_ThrowsArgumentNullException()
        {
            // Arrange
            var tagHelper = SetupTagHelper("TextProperty");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => tagHelper.Process(_context, null!));
        }

        [Fact]
        public void Process_WithTextInput_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("TextProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("text", _output.Attributes["type"].Value);
            Assert.Equal("TextProperty", _output.Attributes["input-id"].Value);
            Assert.Equal("TextProperty", _output.Attributes["name"].Value);
            Assert.Contains("label", _output.Attributes.Select(a => a.Name));
            Assert.Contains("lang", _output.Attributes.Select(a => a.Name));
        }

        [Fact]
        public void Process_WithEmailInput_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("EmailProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("email", _output.Attributes["type"].Value);
            Assert.Equal("EmailProperty", _output.Attributes["input-id"].Value);
            Assert.Equal("EmailProperty", _output.Attributes["name"].Value);
        }

        [Fact]
        public void Process_WithPasswordInput_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("PasswordProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("password", _output.Attributes["type"].Value);
            Assert.Equal("PasswordProperty", _output.Attributes["input-id"].Value);
            Assert.Equal("PasswordProperty", _output.Attributes["name"].Value);
        }

        [Fact]
        public void Process_WithDateInput_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("DateProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-date-input", _output.TagName);
            Assert.Equal("full", _output.Attributes["format"].Value);
        }

        [Fact]
        public void Process_WithCheckbox_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("BoolProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-checkbox", _output.TagName);
            Assert.Equal("BoolProperty", _output.Attributes["checkbox-id"].Value);
        }

        [Fact]
        public void Process_WithTextArea_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("MultilineProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-textarea", _output.TagName);
            Assert.Equal("MultilineProperty", _output.Attributes["textarea-id"].Value);
        }



        [Fact]
        public void Process_WithDateFormat_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("DateWithFormatProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-date-input", _output.TagName);
            Assert.Equal("short", _output.Attributes["format"].Value);
        }

        [Fact]
        public void Process_WithRequiredProperty_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("RequiredTextProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert - Verify basic tag helper functionality works with properties that have validation attributes
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("text", _output.Attributes["type"].Value);
            Assert.Equal("RequiredTextProperty", _output.Attributes["input-id"].Value);
            Assert.Equal("RequiredTextProperty", _output.Attributes["name"].Value);
            Assert.Contains("label", _output.Attributes.Select(a => a.Name));
            Assert.Contains("lang", _output.Attributes.Select(a => a.Name));

            // Note: Validation attributes (like validate-on) are handled by the metadata provider
            // In a real application with proper ASP.NET Core model binding, these would be present
            // This test focuses on verifying the tag helper works with properties that have validation attributes
        }

        [Fact]
        public void Process_AlwaysIncludesRequiredGCDSAttributes()
        {
            // Arrange
            var tagHelper = SetupTagHelper("TextProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert - Verify all required GCDS attributes are present
            Assert.Contains("name", _output.Attributes.Select(a => a.Name));
            Assert.Contains("input-id", _output.Attributes.Select(a => a.Name));
            Assert.Contains("label", _output.Attributes.Select(a => a.Name));
            Assert.Contains("lang", _output.Attributes.Select(a => a.Name));
        }

        [Fact]
        public void Process_WithNumberInput_RendersCorrectly()
        {
            // Arrange
            var tagHelper = SetupTagHelper("NumberProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("number", _output.Attributes["type"].Value);
            Assert.Equal("NumberProperty", _output.Attributes["input-id"].Value);
            Assert.Equal("NumberProperty", _output.Attributes["name"].Value);
        }

        [Fact]
        public void Process_DateTimeProperty_Correctly()
        {

            // Arrange
            var tagHelper = SetupTagHelper(
                "DateTimeProperty",
                new TestModel { DateTimeProperty = new DateTime(2024, 01, 01) }
            );

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("2024-01-01", _output.Attributes["value"].Value);
            Assert.Equal("date", _output.Attributes["type"].Value);
            Assert.Equal("gcds-date-input", _output.TagName);

            //Assert - default when no [DateFormat]
            Assert.Equal("full", _output.Attributes["format"].Value);

        }

        [Fact]
        public void Process_DateTimeProperty_IgnoresTimeComponent()
        {
            // Arrange
            var tagHelper = SetupTagHelper(
                "DateTimeProperty",
                new TestModel
                {
                    // 2024-01-01 at 15:30
                    DateTimeProperty = new DateTime(2024, 1, 1, 15, 30, 0)
                });

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("2024-01-01", _output.Attributes["value"].Value);
            Assert.Equal("date", _output.Attributes["type"].Value);
        }

        [Fact]
        public void Process_WithPhoneDataType_RendersTelInput()
        {
            // Arrange
            var tagHelper = SetupTagHelper("PhoneProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("tel", _output.Attributes["type"].Value);
            Assert.Equal("PhoneProperty", _output.Attributes["input-id"].Value);
        }

        [Fact]
        public void Process_WithImageUrlDataType_RendersUrlInput()
        {
            // Arrange
            var tagHelper = SetupTagHelper("ImageUrlProperty");

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("url", _output.Attributes["type"].Value);
            Assert.Equal("ImageUrlProperty", _output.Attributes["input-id"].Value);
        }

        [Fact]
        public void Process_WithExplicitType_OverridesModelMetadata()
        {
            // Arrange
            var tagHelper = SetupTagHelper("EmailProperty");
            tagHelper.Type = FDCPInputType.search;

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("search", _output.Attributes["type"].Value);
            Assert.Equal("EmailProperty", _output.Attributes["input-id"].Value);
        }

        [Fact]
        public void Process_WithExplicitTextAreaType_RendersTextArea()
        {
            // Arrange
            var tagHelper = SetupTagHelper("TextProperty");
            tagHelper.Type = FDCPInputType.textArea;

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-textarea", _output.TagName);
            Assert.Equal("TextProperty", _output.Attributes["textarea-id"].Value);
            Assert.Equal("TextProperty", _output.Attributes["name"].Value);
        }

        [Fact]
        public void Process_WithManualAttributes_RendersWithoutFor()
        {
            // Arrange
            var tagHelper = new FDCPInputTagHelper
            {
                Name = "SearchTerm",
                Id = "search-term",
                Label = "Search term",
                Hint = "Enter keywords",
                Value = "benefits",
                Type = FDCPInputType.search,
                Required = true,
                Disabled = true,
                ViewContext = new ViewContext()
            };

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.Equal("gcds-input", _output.TagName);
            Assert.Equal("search", _output.Attributes["type"].Value);
            Assert.Equal("Search term", _output.Attributes["label"].Value);
            Assert.Equal("search-term", _output.Attributes["input-id"].Value);
            Assert.Equal("SearchTerm", _output.Attributes["name"].Value);
            Assert.Equal("Enter keywords", _output.Attributes["hint"].Value);
            Assert.Equal("benefits", _output.Attributes["value"].Value);
            Assert.True(_output.Attributes.ContainsName("required"));
            Assert.True(_output.Attributes.ContainsName("disabled"));
        }

        [Fact]
        public void Process_WithRequiredDataAnnotation_OverridesWithFalseRequiredAttribute()
        {
            // Arrange
            var tagHelper = SetupTagHelper("RequiredTextProperty");
            tagHelper.Required = false;

            // Act
            tagHelper.Process(_context, _output);

            // Assert
            Assert.False(_output.Attributes.ContainsName("required"));
        }
    }
}