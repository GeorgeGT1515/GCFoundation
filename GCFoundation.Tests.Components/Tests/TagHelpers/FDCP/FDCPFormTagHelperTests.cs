using GCFoundation.Components.Models;
using GCFoundation.Components.TagHelpers.FDCP;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GCFoundation.Tests.Components.Tests.TagHelpers.FDCP;

public class FDCPFormTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithValidModel_RendersFormCorrectly()
    {
        // Arrange
        var tagHelper = new FDCPFormTagHelper
        {
            Model = new TestViewModel(),
            Method = "post",
            Action = "/test-action"
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.Equal("form", output.TagName);
        Assert.Equal("post", output.Attributes["method"].Value);
        Assert.Equal("/test-action", output.Attributes["action"].Value);
        
        // Verify GCDS v0.39.0+ validation attributes
        Assert.Equal("true", output.Attributes["data-gcds-validation"].Value);
        Assert.Equal("true", output.Attributes["novalidate"].Value);

        var content = output.Content.GetContent();
        Assert.Contains("<gcds-error-summary", content);
        Assert.Contains("lang=", content);
        
        // Verify validation script is included
        var preContent = output.PreContent.GetContent();
        Assert.Contains("gcds-validation-handler.js", preContent);
        Assert.Contains("defer", preContent);
        
        // Error summary should be hidden initially when no errors
        Assert.Contains("class=\"d-none\"", content);
    }

    [Fact]
    public async Task ProcessAsync_WithModelErrors_RendersErrorSummary()
    {
        // Arrange
        var model = new TestViewModel();
        model.AddError("field1", "Error message 1");
        model.AddError("field2", "Error message 2");

        var tagHelper = new FDCPFormTagHelper
        {
            Model = model,
            Method = "post",
            Action = "/test-action"
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        Assert.Contains("error-links", content);

        // Verify error summary is visible when there are server-side errors
        Assert.Contains("class=\"d-block\"", content);

        // Verify error links JSON structure
        var expectedErrors = new Dictionary<string, string>
        {
            { "#field1", "Error message 1" },
            { "#field2", "Error message 2" }
        };
        var expectedJson = JsonSerializer.Serialize(expectedErrors);

        // Extract the error-links attribute value using regex
        var match = Regex.Match(content, "error-links=\"([^\"]*)\"");
        Assert.True(match.Success, "error-links attribute not found in content");

        // HTML-decode the attribute value
        var actualJson = WebUtility.HtmlDecode(match.Groups[1].Value);

        // Compare with expected JSON
        Assert.Equal(expectedJson, actualJson);
    }

    [Fact]
    public async Task ProcessAsync_WithNullAction_OmitsActionAttribute()
    {
        // Arrange
        var tagHelper = new FDCPFormTagHelper
        {
            Model = new TestViewModel(),
            Method = "post",
            Action = string.Empty
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        Assert.False(output.Attributes.ContainsName("action"));
    }

    [Fact]
    public async Task ProcessAsync_WithNullModel_ThrowsInvalidOperationException()
    {
        // Arrange
        var tagHelper = new FDCPFormTagHelper
        {
            Model = null!,
            Method = "post",
            Action = "/test-action"
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => tagHelper.ProcessAsync(context, output));
    }

    [Fact]
    public async Task ProcessAsync_WithComplexValidationErrors_RendersCorrectErrorSummaryFormat()
    {
        // Arrange
        var model = new TestViewModel();
        model.AddError("email", "Please enter a valid email address");
        model.AddError("phone", "Phone number is required");
        model.AddError("nested.field", "Nested field validation error");

        var tagHelper = new FDCPFormTagHelper
        {
            Model = model,
            Method = "post",
            Action = "/test-action"
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        
        // Verify gcds-error-summary structure for GCDS v0.39.0
        Assert.Contains("<gcds-error-summary", content);
        Assert.Contains("error-links", content);
        
        // Verify each error is properly formatted as a link with hash prefix
        // JSON is HTML-encoded in the output
        Assert.Contains("&quot;#email&quot;:", content);
        Assert.Contains("&quot;#phone&quot;:", content);
        Assert.Contains("&quot;#nested.field&quot;:", content);
        
        // Verify error messages are properly escaped
        Assert.Contains("Please enter a valid email address", content);
        Assert.Contains("Phone number is required", content);
        Assert.Contains("Nested field validation error", content);
    }

    [Fact]
    public async Task ProcessAsync_WithSpecialCharactersInErrors_HandlesEncodingCorrectly()
    {
        // Arrange
        var model = new TestViewModel();
        model.AddError("field1", "Error with \"quotes\" and <tags>");
        model.AddError("field2", "Error with & ampersand");

        var tagHelper = new FDCPFormTagHelper
        {
            Model = model,
            Method = "post",
            Action = "/test-action"
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        
        // Extract and validate the error-links JSON
        var match = Regex.Match(content, "error-links=\"([^\"]*)\"");
        Assert.True(match.Success, "error-links attribute not found");
        
        var actualJson = WebUtility.HtmlDecode(match.Groups[1].Value);
        var errors = JsonSerializer.Deserialize<Dictionary<string, string>>(actualJson);
        
        // Verify special characters are properly handled
        Assert.NotNull(errors);
        Assert.Contains("Error with \"quotes\" and <tags>", errors["#field1"]);
        Assert.Contains("Error with & ampersand", errors["#field2"]);
    }

    [Fact]
    public async Task ProcessAsync_WithNoErrors_DoesNotRenderErrorSummary()
    {
        // Arrange
        var model = new TestViewModel(); // No errors added

        var tagHelper = new FDCPFormTagHelper
        {
            Model = model,
            Method = "post",
            Action = "/test-action"
        };

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            "test"
        );

        var output = new TagHelperOutput("fdcp-form",
            new TagHelperAttributeList(),
            (cache, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

        // Act
        await tagHelper.ProcessAsync(context, output);

        // Assert
        var content = output.Content.GetContent();
        
        // For GCDS v0.39.0+, error summary is always rendered but hidden when no errors
        Assert.Contains("<gcds-error-summary", content);
        Assert.Contains("class=\"d-none\"", content);
        
        // Should not have error-links attribute when no errors
        Assert.DoesNotContain("error-links", content);
        
        // Should have validation attributes
        Assert.Equal("true", output.Attributes["data-gcds-validation"].Value);
        Assert.Equal("true", output.Attributes["novalidate"].Value);
    }

    private class TestViewModel : BaseViewModel
    {
        // Test implementation of BaseViewModel
    }
}