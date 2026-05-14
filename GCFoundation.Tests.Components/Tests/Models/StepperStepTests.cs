using GCFoundation.Components.Models;
using GCFoundation.Components.Enums;
namespace GCFoundation.Tests.Components.Tests.Models;
public class StepperStepTests
{
    [Fact]
    public void GetDisplayHtml_WhenIsHidden_ReturnsEmptyString()
    {
        var step = new StepperStep { IsHidden = true, Label = "Step 1" };
        var result = step.GetDisplayHtml(1);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetDisplayHtml_WhenDisplayModeIsNumber_ReturnsStepNumber()
    {
        var step = new StepperStep { StepNumber = 1, DisplayMode = StepperStepDisplayMode.Number, Label = "Step 1" };
        var result = step.GetDisplayHtml(step.StepNumber);
        Assert.Equal(step.StepNumber.ToString(), result);
    }

    [Fact]
    public void GetDisplayHtml_WhenDisplayModeIsIconAndStepCompleted_ReturnsCompletedIconHtml()
    {
        var step = new StepperStep { StepNumber = 1, DisplayMode = StepperStepDisplayMode.Icon, CompletedIconHtml = "<i class='fa fa-check'></i>", Label = "Step 3" };
        var result = step.GetDisplayHtml(3);
        Assert.Equal("<i class='fa fa-check'></i>", result);
    }
    
    [Fact]
    public void GetDisplayHtml_WhenDisplayModeIsIconAndStepActive_ReturnsInProgressIconHtml()
    {
        var step = new StepperStep{ StepNumber = 2, DisplayMode = StepperStepDisplayMode.Icon, InProgressIconHtml = "<i class='fa fa-spinner'></i>", Label = "Step 2" };
        var result = step.GetDisplayHtml(step.StepNumber);
        Assert.Equal("<i class='fa fa-spinner'></i>", result);
    }

    [Fact]
    public void GetDisplayHtml_WhenDisplayModeIsIconAndNoIconSet_ReturnsStepNumber()
    {
        var step = new StepperStep { StepNumber = 1, DisplayMode = StepperStepDisplayMode.Icon, Label = "Step 1" };
        var result = step.GetDisplayHtml(3);
        Assert.Equal(step.StepNumber.ToString(), result);
    }

    [Fact]
    public void GetStatusByCurrentStep_WhenStepNumberLessThanCurrentStep_ReturnsCompleted()
    {
        var step = new StepperStep { StepNumber = 1, Label = "Step 1" };
        var result = step.GetStatusByCurrentStep(2);
        Assert.Equal(StepperStepStatus.completed, result);
    }

    [Fact]
    public void GetStatusByCurrentStep_WhenStepNumberEqualToCurrentStep_ReturnsActive()
    {
        var step = new StepperStep { StepNumber = 2, Label = "Step 2"};
        var result = step.GetStatusByCurrentStep(2);
        Assert.Equal(StepperStepStatus.active, result);
    }

    [Fact]
    public void GetStatusByCurrentStep_WhenStepNumberGreaterThanCurrentStep_ReturnsIncomplete()
    {
        var step = new StepperStep { StepNumber = 3, Label = "Step 3"};
        var result = step.GetStatusByCurrentStep(2);
        Assert.Equal(StepperStepStatus.incomplete, result);
    }
}
