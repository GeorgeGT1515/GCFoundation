namespace GCFoundation.Components.Models
{
    /// <summary>
    /// Represents the configured set of navigation rules used to control node visibility.
    /// </summary>
    public class NavRulesOptions
    {
        /// <summary>
        /// Gets or sets the collection of navigation rules, keyed by navigation node key.
        /// </summary>
        /// <value>A dictionary mapping each navigation node's key to its associated <see cref="NavRule"/>.</value>
        public Dictionary<string, NavRule> Rules { get; } = new();
    }
}