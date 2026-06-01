using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class FileUploadTagHelperTests
    {
        [Fact]
        public void Process_EmitsFileUploadAndCommonFormAttributes()
        {
            var helper = new FileUploadTagHelper
            {
                Label = "Upload documents",
                UploaderId = "documents",
                Name = "DocumentFiles",
                Hint = "PDF only",
                Accept = "application/pdf",
                Multiple = true,
                Disabled = true,
                Required = true
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("Upload documents", output.Attributes["label"].Value?.ToString());
            Assert.Equal("documents", output.Attributes["uploader-id"].Value?.ToString());
            Assert.Equal("DocumentFiles", output.Attributes["name"].Value?.ToString());
            Assert.Equal("PDF only", output.Attributes["hint"].Value?.ToString());
            Assert.Equal("application/pdf", output.Attributes["accept"].Value?.ToString());
            Assert.True(output.Attributes.ContainsName("multiple"));
            Assert.True(output.Attributes.ContainsName("disabled"));
            Assert.True(output.Attributes.ContainsName("required"));
        }

        [Fact]
        public void Process_WithoutOptionalAttributes_DoesNotEmitOptionalAttributes()
        {
            var helper = new FileUploadTagHelper
            {
                Label = "Upload documents",
                UploaderId = "documents",
                Name = "DocumentFiles"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("accept"));
            Assert.False(output.Attributes.ContainsName("multiple"));
            Assert.False(output.Attributes.ContainsName("disabled"));
            Assert.False(output.Attributes.ContainsName("required"));
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-file-upload", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}
