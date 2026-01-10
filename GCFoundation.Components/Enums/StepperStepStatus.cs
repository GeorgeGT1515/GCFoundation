namespace GCFoundation.Components.Enums
{
    /// <summary>  
    /// Defines the status of a step relative to other steps of a stepper.  
    /// </summary>  
    public enum StepperStepStatus
    {
        /// <summary>  
        /// Step before the current step.  
        /// </summary>  
        Completed,

        /// <summary>  
        /// Step is the current step.  
        /// </summary>  
        Active,

        /// <summary>  
        /// Step after the current step.  
        /// </summary>  
        Incomplete
    }
}