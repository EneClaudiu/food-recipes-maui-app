using System.Text.Json.Serialization;

namespace RecipeCabinet.Models
{
    public class TotalNutrients
    {
        [JsonPropertyName("FAT")]
        public NutrientValue TotalFat { get; set; } = new NutrientValue();

        [JsonPropertyName("FASAT")]
        public NutrientValue SaturatedFat { get; set; } = new NutrientValue();

        [JsonPropertyName("FATRN")]
        public NutrientValue TransFat { get; set; } = new NutrientValue();

        [JsonPropertyName("CHOLE")]
        public NutrientValue Cholesterol { get; set; } = new NutrientValue();

        [JsonPropertyName("NA")]
        public NutrientValue Sodium { get; set; } = new NutrientValue();

        [JsonPropertyName("CHOCDF")]
        public NutrientValue Carbohydrate { get; set; } = new NutrientValue();

        [JsonPropertyName("FIBTG")]
        public NutrientValue Fiber { get; set; } = new NutrientValue();

        [JsonPropertyName("SUGAR")]
        public NutrientValue Sugars { get; set; } = new NutrientValue();

        [JsonPropertyName("PROCNT")]
        public NutrientValue Protein { get; set; } = new NutrientValue();

        [JsonPropertyName("VITD")]
        public NutrientValue VitaminD { get; set; } = new NutrientValue();

        [JsonPropertyName("CA")]
        public NutrientValue Calcium { get; set; } = new NutrientValue();

        [JsonPropertyName("FE")]
        public NutrientValue Iron { get; set; } = new NutrientValue();

        [JsonPropertyName("K")]
        public NutrientValue Potassium { get; set; } = new NutrientValue();

        public static TotalNutrients AggregateNutrients(List<NutritionalValues> data)
        {
            return new TotalNutrients
            {
                TotalFat = AggregateNutrient(data, d => d.TotalNutrients.TotalFat),
                SaturatedFat = AggregateNutrient(data, d => d.TotalNutrients.SaturatedFat),
                TransFat = AggregateNutrient(data, d => d.TotalNutrients.TransFat),
                Cholesterol = AggregateNutrient(data, d => d.TotalNutrients.Cholesterol),
                Sodium = AggregateNutrient(data, d => d.TotalNutrients.Sodium),
                Carbohydrate = AggregateNutrient(data, d => d.TotalNutrients.Carbohydrate),
                Fiber = AggregateNutrient(data, d => d.TotalNutrients.Fiber),
                Sugars = AggregateNutrient(data, d => d.TotalNutrients.Sugars),
                Protein = AggregateNutrient(data, d => d.TotalNutrients.Protein),
                VitaminD = AggregateNutrient(data, d => d.TotalNutrients.VitaminD),
                Calcium = AggregateNutrient(data, d => d.TotalNutrients.Calcium),
                Iron = AggregateNutrient(data, d => d.TotalNutrients.Iron),
                Potassium = AggregateNutrient(data, d => d.TotalNutrients.Potassium)
            };
        }

        private static NutrientValue AggregateNutrient(List<NutritionalValues> data, Func<NutritionalValues, NutrientValue> selector)
        {
            var unit = selector(data.First()).Unit;

            return new NutrientValue
            {
                Quantity = data.Sum(d => selector(d).Quantity),
                Unit = unit
            };
        }
    }
}
