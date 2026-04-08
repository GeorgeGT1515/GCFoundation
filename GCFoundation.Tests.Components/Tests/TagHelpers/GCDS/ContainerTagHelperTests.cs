using GCFoundation.Components.Enums;
using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class ContainerTagHelperTests
    {
        [Fact]
        public void Process_WhenCentered_EmitsAlignmentCenter()
        {
            var helper = new ContainerTagHelper { Centered = true };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("center", output.Attributes["alignment"].Value?.ToString());
        }

        [Fact]
        public void Process_WhenMainContainer_EmitsLayoutPage()
        {
            var helper = new ContainerTagHelper { MainContainer = true };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("page", output.Attributes["layout"].Value?.ToString());
        }

        [Fact]
        public void Process_PageLayoutWithCenter_EmitsAlignmentAndLayout()
        {
            var helper = new ContainerTagHelper
            {
                Centered = true,
                MainContainer = true,
                Tag = "main",
                Size = SizeTypeEmum.lg
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("center", output.Attributes["alignment"].Value?.ToString());
            Assert.Equal("page", output.Attributes["layout"].Value?.ToString());
            Assert.Equal("main", output.Attributes["tag"].Value?.ToString());
            Assert.Equal("lg", output.Attributes["size"].Value?.ToString());
        }

        [Fact]
        public void Process_DoesNotEmitLegacyCenteredOrMainContainerAttributes()
        {
            var helper = new ContainerTagHelper { Centered = true, MainContainer = true };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("centered"));
            Assert.False(output.Attributes.ContainsName("main-container"));
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-container", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
