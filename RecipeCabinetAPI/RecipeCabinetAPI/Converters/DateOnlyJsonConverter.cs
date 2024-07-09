using System.Text.Json.Serialization;
using System.Text.Json;

namespace RecipeCabinetAPI.Converters
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private readonly string _format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateTime = DateTime.Parse(reader.GetString());
            return DateOnly.FromDateTime(dateTime);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format));
        }
    }
}
