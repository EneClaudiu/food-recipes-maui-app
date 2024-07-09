using System.Text.Json.Serialization;

namespace RecipeCabinet.Models
{
    public class NutritionalValues
    {
        [JsonPropertyName("calories")]
        public double Calories { get; set; }

        [JsonPropertyName("totalNutrients")]
        public TotalNutrients TotalNutrients { get; set; }

        [JsonPropertyName("totalDaily")]
        public TotalDaily TotalDaily { get; set; }

        [JsonConstructor]
        public NutritionalValues(double calories, TotalNutrients totalNutrients = null, TotalDaily totalDaily = null)
        {
            Calories = calories;
            TotalNutrients = totalNutrients ?? new TotalNutrients();
            TotalDaily = totalDaily ?? new TotalDaily();
        }

        public static NutritionalValues AggregateNutritionData(List<NutritionalValues> data)
        {
            return new NutritionalValues(data.Sum(d => d.Calories), TotalNutrients.AggregateNutrients(data), TotalDaily.AggregateDailyValues(data));
        }

        public void RoundQuantities()
        {
            Calories = Math.Round(Calories, 1);
            RoundNutrientQuantities(TotalNutrients);
            RoundDailyValueQuantities(TotalDaily);
        }

        private void RoundNutrientQuantities(TotalNutrients nutrients)
        {
            if (nutrients != null)
            {
                if (nutrients.TotalFat != null) nutrients.TotalFat.Quantity = Math.Round(nutrients.TotalFat.Quantity, 1);
                if (nutrients.SaturatedFat != null) nutrients.SaturatedFat.Quantity = Math.Round(nutrients.SaturatedFat.Quantity, 1);
                if (nutrients.TransFat != null) nutrients.TransFat.Quantity = Math.Round(nutrients.TransFat.Quantity, 1);
                if (nutrients.Cholesterol != null) nutrients.Cholesterol.Quantity = Math.Round(nutrients.Cholesterol.Quantity, 1);
                if (nutrients.Sodium != null) nutrients.Sodium.Quantity = Math.Round(nutrients.Sodium.Quantity, 1);
                if (nutrients.Carbohydrate != null) nutrients.Carbohydrate.Quantity = Math.Round(nutrients.Carbohydrate.Quantity, 1);
                if (nutrients.Fiber != null) nutrients.Fiber.Quantity = Math.Round(nutrients.Fiber.Quantity, 1);
                if (nutrients.Sugars != null) nutrients.Sugars.Quantity = Math.Round(nutrients.Sugars.Quantity, 1);
                if (nutrients.Protein != null) nutrients.Protein.Quantity = Math.Round(nutrients.Protein.Quantity, 1);
                if (nutrients.VitaminD != null) nutrients.VitaminD.Quantity = Math.Round(nutrients.VitaminD.Quantity, 1);
                if (nutrients.Calcium != null) nutrients.Calcium.Quantity = Math.Round(nutrients.Calcium.Quantity, 1);
                if (nutrients.Iron != null) nutrients.Iron.Quantity = Math.Round(nutrients.Iron.Quantity, 1);
                if (nutrients.Potassium != null) nutrients.Potassium.Quantity = Math.Round(nutrients.Potassium.Quantity, 1);
            }
        }

        private void RoundDailyValueQuantities(TotalDaily dailyValues)
        {
            if (dailyValues != null)
            {
                if (dailyValues.TotalFat != null) dailyValues.TotalFat.Quantity = Math.Round(dailyValues.TotalFat.Quantity, 1);
                if (dailyValues.SaturatedFat != null) dailyValues.SaturatedFat.Quantity = Math.Round(dailyValues.SaturatedFat.Quantity, 1);
                if (dailyValues.Cholesterol != null) dailyValues.Cholesterol.Quantity = Math.Round(dailyValues.Cholesterol.Quantity, 1);
                if (dailyValues.Sodium != null) dailyValues.Sodium.Quantity = Math.Round(dailyValues.Sodium.Quantity, 1);
                if (dailyValues.Carbohydrate != null) dailyValues.Carbohydrate.Quantity = Math.Round(dailyValues.Carbohydrate.Quantity, 1);
                if (dailyValues.Fiber != null) dailyValues.Fiber.Quantity = Math.Round(dailyValues.Fiber.Quantity, 1);
                if (dailyValues.Protein != null) dailyValues.Protein.Quantity = Math.Round(dailyValues.Protein.Quantity, 1);
                if (dailyValues.VitaminD != null) dailyValues.VitaminD.Quantity = Math.Round(dailyValues.VitaminD.Quantity, 1);
                if (dailyValues.Calcium != null) dailyValues.Calcium.Quantity = Math.Round(dailyValues.Calcium.Quantity, 1);
                if (dailyValues.Iron != null) dailyValues.Iron.Quantity = Math.Round(dailyValues.Iron.Quantity, 1);
                if (dailyValues.Potassium != null) dailyValues.Potassium.Quantity = Math.Round(dailyValues.Potassium.Quantity, 1);
            }
        }
    }
}
