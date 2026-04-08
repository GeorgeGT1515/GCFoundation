using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    /// <summary>
    /// Tests for <see cref="CardTagHelper"/> (GCDS <c>gcds-card</c>), including <c>card-title-tag</c> heading tokens.
    /// </summary>
    public class GcdsCardTagHelperTests
    {
        [Theory]
        [InlineData(CardTitleTag.h3, "h3")]
        [InlineData(CardTitleTag.h4, "h4")]
        [InlineData(CardTitleTag.h5, "h5")]
        [InlineData(CardTitleTag.h6, "h6")]
        public void Process_SetsCardTitleTag(CardTitleTag tag, string expected)
        {
            var helper = new CardTagHelper
            {
                CardTitle = "Title",
                CardTitleTag = tag,
                Href = "/detail"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("Title", output.Attributes["card-title"].Value?.ToString());
            Assert.Equal(expected, output.Attributes["card-title-tag"].Value?.ToString());
            Assert.Equal("/detail", output.Attributes["href"].Value?.ToString());
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-card", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
