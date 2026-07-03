using GCFoundation.Components.Enums;

namespace GCFoundation.Components.Models
{
    /// <summary>
    /// Represents a single rule that governs the visibility of a navigation node.
    /// </summary>
    public class NavRule
    {
        /// <summary>
        /// Gets or sets the type of condition used to evaluate this rule (e.g. cookie, session).
        /// </summary>
        /// <value>The rule type. Default is <see cref="NavRuleType.Cookie"/>.</value>
        public NavRuleType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the cookie or session value to check.
        /// </summary>
        /// <value>The name used to look up the value on the current request.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value that the resolved cookie or session value must match
        /// for the associated navigation node to be shown.
        /// </summary>
        /// <value>The expected value used for the comparison.</value>
        public string Value { get; set; }
    }
}