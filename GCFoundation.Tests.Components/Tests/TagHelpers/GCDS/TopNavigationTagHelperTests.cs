using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class TopNavigationTagHelperTests
    {
        [Theory]
        [InlineData(TopMenuAlignment.end, "end")]
        [InlineData(TopMenuAlignment.start, "start")]
        public void Process_SetsAlignmentUsingGcdsV1Tokens(TopMenuAlignment alignment, string expected)
        {
            var helper = new TopNavigationTagHelper
            {
                Label = "Menu",
                Alignment = alignment
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal(expected, output.Attributes["alignment"].Value?.ToString());
            Assert.Equal("Menu", output.Attributes["label"].Value?.ToString());
        }

        [Fact]
        public void Process_DefaultAlignment_IsEnd()
        {
            var helper = new TopNavigationTagHelper { Label = "Nav" };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("end", output.Attributes["alignment"].Value?.ToString());
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-top-nav", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
