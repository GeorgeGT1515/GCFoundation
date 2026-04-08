using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class HeaderTagHelperTests
    {
        [Fact]
        public void Process_DoesNotEmitSignatureVariant_GcdsV1RemovedAttribute()
        {
            var helper = new HeaderTagHelper
            {
                LangHref = "/fr",
                SkipToHref = "#main"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("signature-variant"));
            Assert.Equal("/fr", output.Attributes["lang-href"].Value?.ToString());
            Assert.Equal("#main", output.Attributes["skip-to-href"].Value?.ToString());
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-header", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
