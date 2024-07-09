using System.Text.Json.Serialization;

namespace RecipeCabinet.Models
{
    public class NutrientValue
    {
        [JsonPropertyName("quantity")]
        public double Quantity { get; set; } = 0;

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;
    }
}
