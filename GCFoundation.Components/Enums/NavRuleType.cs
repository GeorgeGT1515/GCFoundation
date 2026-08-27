namespace GCFoundation.Components.Enums
{
    /// <summary>
    /// Specifies the type of condition used to evaluate a navigation rule.
    /// </summary>
    public enum NavRuleType
    {
        /// <summary>
        /// The rule is evaluated against a claim on the current authenticated user.
        /// </summary>
        Claim,

        /// <summary>
        /// The rule is evaluated against a cookie value on the current request.
        /// </summary>
        Cookie,

        /// <summary>
        /// The rule is evaluated against a session value on the current request.
        /// </summary>
        Session
    }
}