using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class CheckboxesTagHelperTests
    {
        [Fact]
        public void Process_WithManualFieldAttributes_EmitsResolvedAttributes()
        {
            var helper = new CheckboxesTagHelper
            {
                Legend = "Choose options",
                Name = "Choices",
                Hint = "Select all that apply",
                Value = "a,b",
                Options = "[{\"label\":\"A\",\"value\":\"a\"}]"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("gcds-checkboxes", output.TagName);
            Assert.Equal("Choices", output.Attributes["name"].Value?.ToString());
            Assert.Equal("Choose options", output.Attributes["legend"].Value?.ToString());
            Assert.Equal("Select all that apply", output.Attributes["hint"].Value?.ToString());
            Assert.Equal("a,b", output.Attributes["value"].Value?.ToString());
            Assert.Equal("[{\"label\":\"A\",\"value\":\"a\"}]", output.Attributes["options"].Value?.ToString());
        }

        [Fact]
        public void Process_WithoutForOrName_ThrowsInvalidOperationException()
        {
            var helper = new CheckboxesTagHelper
            {
                Legend = "Choose options",
                Options = "[]"
            };

            var output = CreateOutput();
            var exception = Assert.Throws<InvalidOperationException>(() => helper.Process(CreateContext(), output));

            Assert.Contains("Either 'for' or 'name' must be specified", exception.Message);
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-checkboxes", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
