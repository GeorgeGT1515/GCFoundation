using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class DateInputTagHelperTests
    {
        [Fact]
        public void Process_EmitsDateInputAndCommonFormAttributes()
        {
            var helper = new DateInputTagHelper
            {
                Format = DateInputFormatType.full,
                Legend = "Date of birth",
                Name = "DateOfBirth",
                Hint = "Use the format shown.",
                Required = true,
                Value = "2026-06-01"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("full", output.Attributes["format"].Value?.ToString());
            Assert.Equal("Date of birth", output.Attributes["legend"].Value?.ToString());
            Assert.Equal("DateOfBirth", output.Attributes["name"].Value?.ToString());
            Assert.Equal("Use the format shown.", output.Attributes["hint"].Value?.ToString());
            Assert.Equal("2026-06-01", output.Attributes["value"].Value?.ToString());
            Assert.True(output.Attributes.ContainsName("required"));
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-date-input", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
