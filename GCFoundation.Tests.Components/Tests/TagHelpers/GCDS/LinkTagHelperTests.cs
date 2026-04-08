using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class LinkTagHelperTests
    {
        [Theory]
        [InlineData(LinkVariant.Default, "default")]
        [InlineData(LinkVariant.Light, "light")]
        public void Process_SetsLinkRoleFromVariant(LinkVariant variant, string expectedRole)
        {
            var helper = new LinkTagHelper
            {
                Href = "/path",
                Target = "_self",
                Variant = variant
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal(expectedRole, output.Attributes["link-role"].Value?.ToString());
            Assert.Equal("/path", output.Attributes["href"].Value?.ToString());
        }

        [Fact]
        public void Process_DoesNotEmitLegacyVariantAttributeName()
        {
            var helper = new LinkTagHelper
            {
                Href = "/x",
                Target = "_blank",
                Variant = LinkVariant.Light
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("variant"));
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-link", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
