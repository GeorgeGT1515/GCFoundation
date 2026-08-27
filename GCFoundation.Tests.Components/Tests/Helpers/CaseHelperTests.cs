using GCFoundation.Components.Helpers;
namespace GCFoundation.Tests.Components.Tests.Helpers;

public class CaseHelperTests
{
    [Fact]
    public void ConvertToKebabCase_WhenInputIsNull_ReturnsNull()
    {
        var result = CaseHelper.ConvertToKebabCase(null!);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertToKebabCase_WhenInputIsEmpty_ReturnsEmpty()
    {
        var result = CaseHelper.ConvertToKebabCase(string.Empty);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ConvertToKebabCase_WhenPascalCase_ReturnsKebabCase()
    {
        var result = CaseHelper.ConvertToKebabCase("PascalCase");
        Assert.Equal("pascal-case", result);
    }

    [Fact]
    public void ConvertToKebabCase_WhenCamelCase_ReturnsKebabCase()
    {
        var result = CaseHelper.ConvertToKebabCase("camelCase");
        Assert.Equal("camel-case", result);
    }

    [Fact]
    public void ConvertToKebabCase_WhenLowercase_ReturnsUnchanged()
    {
        var result = CaseHelper.ConvertToKebabCase("lowercase");
        Assert.Equal("lowercase", result);
    } 
    
}