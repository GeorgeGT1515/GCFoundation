namespace GCFoundation.Components.Enums
{
    /// <summary>
    /// Defines the available background colours for components.
    /// </summary>
    public enum BackgroundColour
    {
        /// <summary>
        /// Dark background colour. Use with a text shade of 100 or lighter (like --gcds-text-light).
        /// </summary>
        dark,

        /// <summary>
        /// Light background colour (alternative to white). Use with a text shade of 700 or darker (like --gcds-text-primary).
        /// </summary>
        light,

        /// <summary>
        /// Primary background colour. Use with a text shade of 100 or lighter (like --gcds-text-light).
        /// </summary>
        primary,

        /// <summary>
        /// White background colour. Use with a text shade of 700 or darker (like --gcds-text-primary).
        /// </summary>
        white
    }
}
