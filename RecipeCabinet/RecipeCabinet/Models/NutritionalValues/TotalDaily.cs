using System.Text.Json.Serialization;

namespace RecipeCabinet.Models
{
    public class TotalDaily
    {
        [JsonPropertyName("FAT")]
        public NutrientValue TotalFat { get; set; } = new NutrientValue();

        [JsonPropertyName("FASAT")]
        public NutrientValue SaturatedFat { get; set; } = new NutrientValue();

        [JsonPropertyName("CHOLE")]
        public NutrientValue Cholesterol { get; set; } = new NutrientValue();

        [JsonPropertyName("NA")]
        public NutrientValue Sodium { get; set; } = new NutrientValue();

        [JsonPropertyName("CHOCDF")]
        public NutrientValue Carbohydrate { get; set; } = new NutrientValue();

        [JsonPropertyName("FIBTG")]
        public NutrientValue Fiber { get; set; } = new NutrientValue();

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

        public static TotalDaily AggregateDailyValues(List<NutritionalValues> data)
        {
            return new TotalDaily
            {
                TotalFat = AggregateDailyValue(data, d => d.TotalDaily.TotalFat),
                SaturatedFat = AggregateDailyValue(data, d => d.TotalDaily.SaturatedFat),
                Cholesterol = AggregateDailyValue(data, d => d.TotalDaily.Cholesterol),
                Sodium = AggregateDailyValue(data, d => d.TotalDaily.Sodium),
                Carbohydrate = AggregateDailyValue(data, d => d.TotalDaily.Carbohydrate),
                Fiber = AggregateDailyValue(data, d => d.TotalDaily.Fiber),
                Protein = AggregateDailyValue(data, d => d.TotalDaily.Protein),
                VitaminD = AggregateDailyValue(data, d => d.TotalDaily.VitaminD),
                Calcium = AggregateDailyValue(data, d => d.TotalDaily.Calcium),
                Iron = AggregateDailyValue(data, d => d.TotalDaily.Iron),
                Potassium = AggregateDailyValue(data, d => d.TotalDaily.Potassium)
            };
        }

        private static NutrientValue AggregateDailyValue(List<NutritionalValues> data, Func<NutritionalValues, NutrientValue> selector)
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
