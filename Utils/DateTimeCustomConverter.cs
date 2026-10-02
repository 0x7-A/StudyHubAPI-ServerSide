using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StudyHubAPI.Utils
{
    public class DateTimeCustomConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateString))
                return default;

            // 1. Try standard ISO 8601 parsing (Handles Swagger 'T', milliseconds, and 'Z')
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var parsedDate))
            {
                return parsedDate;
            }

            // 2. Fallback to your custom format "yyyy-MM-dd HH:mm:ss"
            if (DateTime.TryParseExact(dateString, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exactDate))
            {
                return exactDate;
            }

            throw new JsonException($"Unable to parse '{dateString}' as a valid DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}