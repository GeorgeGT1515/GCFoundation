namespace GCFoundation.Components.Enums
{
    /// <summary>
    /// Defines the input type based on how the input will validate the values a person enters.
    /// The input <c>type</c> attribute accepts the following options:
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// A single checkbox input to collect a boolean value, like yes/no or true/false.
        /// </summary>
        checkbox,

        /// <summary>
        /// An input to collect a date.
        /// </summary>
        date,

        /// <summary>
        /// An input to collect an email address.
        /// </summary>
        email,

        /// <summary>
        /// An input to collect whole numbers that can increase or decrease, like quantities.
        /// </summary>
        /// <remarks>Tip: When you set a constraint, like a numerical range, use hint text to relay a precise instruction.</remarks>
        number,

        /// <summary>
        /// An input to collect passwords.
        /// </summary>
        /// <remarks>Tip: Communicate any password constraints in the hint text for the input so the person choosing the password knows exactly what the rules are.</remarks>
        password,

        /// <summary>
        /// An input to collect search queries.
        /// </summary>
        search,

        /// <summary>
        /// An input to collect phone numbers.
        /// </summary>
        tel,

        /// <summary>
        /// An input that doesn't fit any other specific input type - for single-line entry.
        /// </summary>
        text,

        /// <summary>
        /// An input that doesn't fit any other specific input type - for multi-line entry.
        /// </summary>
        textArea,

        /// <summary>
        /// An input to collect a URL, like a web address or domain name.
        /// </summary>
        url
    }
}