using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.FDCP;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.FDCP
{
    public class FDCPBadgeTagHelperTests
    {
        [Fact]
        public async Task ProcessAsync_BasicBadge_RendersCorrectly()
        {
            // Arrange
            var helper = new FDCPBadgeTagHelper
            {
                Style = BadgeStyle.primary
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-unique-id");

            var output = new TagHelperOutput("fdcp-badge",
                new TagHelperAttributeList(),
                (_, __) =>
                {
                    return Task.FromResult(new DefaultTagHelperContent().SetContent("Test Content"));
                });

            // Act
            await helper.ProcessAsync(context, output);

            // Assert
            Assert.Equal("span", output.TagName);
            Assert.Contains("fdcp-badge fdcp-badge-primary", output.Attributes["class"].Value.ToString());
            Assert.Contains("<span class='fdcp-badge-content'>Test Content</span>", output.Content.GetContent());
        }

        [Fact]
        public async Task ProcessAsync_WithStartAndEndContent_RendersAllSections()
        {
            // Arrange
            var helper = new FDCPBadgeTagHelper
            {
                Style = BadgeStyle.info,
                StartContent = "Start",
                EndContent = "End"
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-unique-id");

            var output = new TagHelperOutput("fdcp-badge",
                new TagHelperAttributeList(),
                (_, __) =>
                {
                    return Task.FromResult(new DefaultTagHelperContent().SetContent("Middle"));
                });

            // Act
            await helper.ProcessAsync(context, output);

            // Assert
            var content = output.Content.GetContent();
            Assert.Contains("<span class='fdcp-badge-start'>Start</span>", content);
            Assert.Contains("<span class='fdcp-badge-content'>Middle</span>", content);
            Assert.Contains("<span class='fdcp-badge-end'>End</span>", content);
        }

        [Fact]
        public async Task ProcessAsync_WithSlots_PrefersSlotsOverProps()
        {
            // Arrange
            var helper = new FDCPBadgeTagHelper
            {
                Style = BadgeStyle.warning,
                StartContent = "PropStart",
                EndContent = "PropEnd"
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-unique-id");

            var output = new TagHelperOutput("fdcp-badge",
                new TagHelperAttributeList(),
                (_, __) =>
                {
                    return Task.FromResult(
                        new DefaultTagHelperContent().SetHtmlContent(
                            "<div slot='start-content'>SlotStart</div>" +
                            "Middle" +
                            "<div slot='end-content'>SlotEnd</div>"
                        ));
                });

            // Act
            await helper.ProcessAsync(context, output);

            // Assert
            var content = output.Content.GetContent();
            Assert.Contains("<span class='fdcp-badge-start'>SlotStart</span>", content);
            Assert.Contains("<span class='fdcp-badge-content'>Middle</span>", content);
            Assert.Contains("<span class='fdcp-badge-end'>SlotEnd</span>", content);
        }

        [Fact]
        public async Task ProcessAsync_WithInverted_AddsInvertedClass()
        {
            // Arrange
            var helper = new FDCPBadgeTagHelper
            {
                Style = BadgeStyle.success,
                Inverted = true
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-unique-id");

            var output = new TagHelperOutput("fdcp-badge",
                new TagHelperAttributeList(),
                (_, __) => Task.FromResult(new DefaultTagHelperContent().SetContent("Test")));

            // Act
            await helper.ProcessAsync(context, output);

            // Assert
            Assert.Contains("inverted", output.Attributes["class"].Value.ToString());
        }

        [Theory]
        [InlineData(BadgeStyle.primary, "fdcp-badge-primary")]
        [InlineData(BadgeStyle.secondary, "fdcp-badge-secondary")]
        [InlineData(BadgeStyle.success, "fdcp-badge-success")]
        [InlineData(BadgeStyle.danger, "fdcp-badge-danger")]
        [InlineData(BadgeStyle.info, "fdcp-badge-info")]
        [InlineData(BadgeStyle.warning, "fdcp-badge-warning")]
        [InlineData(BadgeStyle.light, "fdcp-badge-light")]
        [InlineData(BadgeStyle.dark, "fdcp-badge-dark")]
        public async Task ProcessAsync_WithDifferentStyles_RendersCorrectClass(BadgeStyle style, string expectedClass)
        {
            // Arrange
            var helper = new FDCPBadgeTagHelper { Style = style };
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-unique-id");

            var output = new TagHelperOutput("fdcp-badge",
                new TagHelperAttributeList(),
                (_, __) => Task.FromResult(new DefaultTagHelperContent().SetContent("Test")));

            // Act
            await helper.ProcessAsync(context, output);

            // Assert
            Assert.Contains(expectedClass, output.Attributes["class"].Value.ToString());
        }

        [Fact]
        public async Task ProcessAsync_WithTagId_RendersIdAttribute()
        {
            // Arrange
            var helper = new FDCPBadgeTagHelper
            {
                Style = BadgeStyle.primary,
                TagId = "test-id"
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                "test-unique-id");

            var output = new TagHelperOutput("fdcp-badge",
                new TagHelperAttributeList(),
                (_, __) => Task.FromResult(new DefaultTagHelperContent().SetContent("Test")));

            // Act
            await helper.ProcessAsync(context, output);

            // Assert
            Assert.Equal("test-id", output.Attributes["id"].Value);
        }
    }
}