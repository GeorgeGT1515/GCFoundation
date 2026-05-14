using GCFoundation.Components.Models;
using System.ComponentModel.DataAnnotations;
namespace GCFoundation.Tests.Components.Tests.Models;
public class BaseViewModelTests
{
    private class TestModel : BaseViewModel
    {
        [Required]
        public string? Name { get; set; } = null;
    }

    [Fact]
    public void Validate_WhenModelIsInvalid_PopulateErrors()
    {
        var model = new TestModel();
        model.Validate();
        Assert.Contains("Name", model.Errors.Keys);
    }

    [Fact]
    public void Validate_WhenModelIsValid_LeavesErrorsEmpty()
    {
        var model = new BaseViewModel();
        model.Validate();
        Assert.Empty(model.Errors);
    }

    [Fact]
    public void Validate_WhenCalled_ClearsPreviousErrors()
    {
        var model = new BaseViewModel();
        model.AddError("Name", "Name is required");
        model.Validate();
        Assert.Empty(model.Errors);
    }

    [Fact]
    public void AddErrors_WhenFieldHasError_AddsErrormessage()
    {
        var model = new BaseViewModel();
        model.AddError("Name", "Name is required");
        Assert.Contains("Name is required", model.Errors["Name"]);
    }

    [Fact]
    public void AddErrors_WhenSameFieldCalledTwice_AllowsMultipleErrors()
    {
        var model = new BaseViewModel();
        model.AddError("Name", "name is required");
        model.AddError("Name", "name is invalid");
        Assert.Contains("name is required", model.Errors["Name"]);
        Assert.Contains("name is invalid", model.Errors["Name"]);
    }

    [Fact]
    public void IsValid_WhenNoErrors_ReturnsTrue() 
    {
        var model = new BaseViewModel();
        Assert.True(model.IsValid);
    }

    [Fact]
    public void IsValid_WhenErrors_ReturnsFalse()
    {
        var model = new BaseViewModel();
        model.AddError("Name", "Name is required");
        Assert.False(model.IsValid);
    }


    [Fact]
    public void ClearErrors_WhenCalled_ClearsAllErrors()
    {
        var model = new BaseViewModel();
        model.AddError("Name", "name is required");
        model.ClearErrors();
        Assert.Empty(model.Errors);
    }
}