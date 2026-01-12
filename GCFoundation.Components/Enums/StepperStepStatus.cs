namespace GCFoundation.Components.Enums
{
    /// <summary>  
    /// Defines the status of a step relative to other steps of a stepper.  
    /// </summary>  
    public enum StepperStepStatus
    {
        /// <summary>  
        /// Step is the current step.  
        /// </summary>  
        active,

        /// <summary>  
        /// Step is before the current step.  
        /// </summary>  
        completed,

        /// <summary>  
        /// Step is after the current step.  
        /// </summary>  
        incomplete
    }
}