namespace GCFoundation.Components.Enums
{
    /// <summary>
    /// Defines the available text colours for the text components.
    /// </summary>
    public enum TextColour
    {
        /// <summary>
        /// Main light text colour. Use on a background shade of 700 or darker (like --gcds-bg-dark).
        /// </summary>
        light,

        /// <summary>
        /// Main text colour. Use on a background shade of 50 or lighter (like --gcds-bg-white).
        /// </summary>
        primary,

        /// <summary>
        /// Contrast text colour (alternative to primary). Use on a background shade of 50 or lighter (like --gcds-bg-white).
        /// </summary>
        secondary
    }
}
