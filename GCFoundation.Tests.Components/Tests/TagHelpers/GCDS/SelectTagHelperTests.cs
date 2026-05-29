using GCFoundation.Components.TagHelpers.GCDS;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.GCDS
{
    public class SelectTagHelperTests
    {
        [Fact]
        public void Process_EmitsAutocomplete_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                Autocomplete = "on"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("on", output.Attributes["autocomplete"].Value?.ToString());
        }

        [Fact]
        public void Process_DoesNotEmitAutocomplete_WhenNotSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("autocomplete"));
        }

        [Fact]
        public void Process_EmitsDisabled_WhenDisabledIsTrue()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                Disabled = true
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.True(output.Attributes.ContainsName("disabled"));
        }

        [Fact]
        public void Process_DoesNotEmitDisabled_WhenDisabledIsFalse()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                Disabled = false
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("disabled"));
        }

        [Fact]
        public void Process_DoesNotEmitDisabled_WhenNotSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("disabled"));
        }

        [Fact]
        public void Process_EmitsErrorMessage_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                ErrorMessage = "Error message"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("Error message", output.Attributes["error-message"].Value?.ToString());
        }

        [Fact]
        public void Process_DoesNotEmitErrorMessage_WhenNotSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("error-message"));
        }

        [Fact]
        public void Process_EmitsHint_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                Hint = "Hint message"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("Hint message", output.Attributes["hint"].Value?.ToString());
        }

        [Fact]
        public void Process_DoesNotEmitHint_WhenNotSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.False(output.Attributes.ContainsName("hint"));
        }

        [Fact]
        public void Process_EmitsRequired_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                Required = true
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.True(output.Attributes.ContainsName("required"));
        }

        [Fact]
        public void Process_EmitsDefaultValue_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                DefaultValue = "Select option"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("Select option", output.Attributes["default-value"].Value?.ToString());
        }

        [Fact]
        public void Process_EmitsValue_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                Value = "2"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("2", output.Attributes["value"].Value?.ToString());
        }

        [Fact]
        public void Process_EmitsValidateOn_WhenSet()
        {
            var helper = new SelectTagHelper
            {
                Label = "Test",
                SelectId = "test-select",
                Name = "test",
                ValidateOn = "other"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("other", output.Attributes["validate-on"].Value?.ToString());
        }

        [Fact]
        public void Process_EmitsLabelAndSelectId()
        {
            var helper = new SelectTagHelper
            {
                Label = "My Label",
                SelectId = "my-select",
                Name = "my-select"
            };

            var output = CreateOutput();
            helper.Process(CreateContext(), output);

            Assert.Equal("My Label", output.Attributes["label"].Value?.ToString());
            Assert.Equal("my-select", output.Attributes["select-id"].Value?.ToString());
        }

        private static TagHelperContext CreateContext() =>
            new(new TagHelperAttributeList(), new Dictionary<object, object>(), "test-id");

        private static TagHelperOutput CreateOutput() =>
            new("gcds-select", new TagHelperAttributeList(),
                (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }
}