using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class NoticeTagHelperTests
    {
        [Theory]
        [InlineData(AlertType.info, "info")]
        [InlineData(AlertType.warning, "warning")]
        [InlineData(AlertType.danger, "danger")]
        [InlineData(AlertType.success, "success")]
        public void Process_SetsNoticeRoleFromAlertType(AlertType type, string expectedRole)
        {
            var helper = new NoticeTagHelper
            {
                Title = "Test title",
                Type = type,
                TitleTag = HeadingTag.h3
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("gcds-notice", output.TagName);
            Assert.Equal("Test title", output.Attributes["notice-title"].Value?.ToString());
            Assert.Equal("h3", output.Attributes["notice-title-tag"].Value?.ToString());
            Assert.Equal(expectedRole, output.Attributes["notice-role"].Value?.ToString());
        }

        [Fact]
        public void Process_DoesNotEmitLegacyTypeAttribute()
        {
            var helper = new NoticeTagHelper
            {
                Title = "T",
                Type = AlertType.warning
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("type"));
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-notice", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
