using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class RadiosTagHelperTests
    {
        [Fact]
        public void Process_WithManualFieldAttributes_EmitsResolvedAttributes()
        {
            var helper = new RadiosTagHelper
            {
                Legend = "Choose one",
                Name = "Choice",
                Hint = "Select the best option",
                Value = "b",
                Options = "[{\"label\":\"B\",\"value\":\"b\"}]"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("gcds-radios", output.TagName);
            Assert.Equal("Choice", output.Attributes["name"].Value?.ToString());
            Assert.Equal("Choose one", output.Attributes["legend"].Value?.ToString());
            Assert.Equal("Select the best option", output.Attributes["hint"].Value?.ToString());
            Assert.Equal("b", output.Attributes["value"].Value?.ToString());
            Assert.Equal("[{\"label\":\"B\",\"value\":\"b\"}]", output.Attributes["options"].Value?.ToString());
        }

        [Fact]
        public void Process_WithoutForOrName_ThrowsInvalidOperationException()
        {
            var helper = new RadiosTagHelper
            {
                Legend = "Choose one",
                Options = "[]"
            };

            var output = CreateOutput();
            var exception = Assert.Throws<InvalidOperationException>(() => helper.Process(CreateContext(), output));

            Assert.Contains("Either 'for' or 'name' must be specified", exception.Message);
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-radios", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
