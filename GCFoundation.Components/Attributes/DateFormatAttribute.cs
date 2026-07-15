using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GCFoundation.Components.Attributes
{
    /// <summary>
    /// Specifies the format for a date property in a class.
    /// This attribute can be applied to properties to indicate how the date should be formatted when displayed or serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DateFormatAttribute : JsonConverterAttribute
    {
        /// <summary>
        /// Gets the date format string.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFormatAttribute"/> class with the specified date format.
        /// </summary>
        /// <param name="format">The date format string to be applied to the property.</param>
        public DateFormatAttribute(string format)
        {
            Format = format;
        }

        /// <summary>
        /// Creates a converter for the specified type using the provided date format.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            // Instantiate the converter factory
            return (JsonConverter)Activator.CreateInstance(typeof(DateFormatConverterFactory<>).MakeGenericType(typeToConvert), Format);
        }
    }

    /// <summary>
    /// A factory for creating JSON converters that format date properties according to a specified format string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateFormatConverterFactory<T> : JsonConverterFactory
    {
        private readonly string _format;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFormatConverterFactory{T}"/> class with the specified date format.
        /// </summary>
        /// <param name="format"></param>
        public DateFormatConverterFactory(string format)
        {
            _format = format;
        }

        /// <summary>
        /// Determines whether the converter can be used to convert the specified type.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert) => true;

        /// <summary>
        /// Creates a converter for the specified type using the provided date format.
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Create the actual converter with the format string
            return (JsonConverter)Activator.CreateInstance(typeof(DateFormatConverter<T>), _format);
        }
    }

    /// <summary>
    /// A JSON converter that formats date properties according to a specified format string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DateFormatConverter<T> : JsonConverter<T>
    {
        private readonly string _format;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFormatConverter{T}"/> class with the specified date format.
        /// </summary>
        /// <param name="format"></param>
        public DateFormatConverter(string format)
        {
            _format = format;
        }

        /// <summary>
        /// Reads and converts the JSON to the specified type, applying the date format - if applicable.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateStr = reader.GetString();
            if (dateStr == null)
                return default;

            if (typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?))
                return (T)(object)DateTime.ParseExact(dateStr, _format, CultureInfo.InvariantCulture);
            else if (typeToConvert == typeof(DateTimeOffset) || typeToConvert == typeof(DateTimeOffset?))
                return (T)(object)DateTimeOffset.ParseExact(dateStr, _format, CultureInfo.InvariantCulture);

            throw new NotSupportedException($"Type {typeToConvert} not supported.");
        }

        /// <summary>
        /// Writes a value as JSON, applying the specified date format - if applicable.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value is DateTime dt)
                writer.WriteStringValue(dt.ToString(_format, CultureInfo.InvariantCulture));
            else if (value is DateTimeOffset dto)
                writer.WriteStringValue(dto.ToString(_format, CultureInfo.InvariantCulture));
        }
    }
}