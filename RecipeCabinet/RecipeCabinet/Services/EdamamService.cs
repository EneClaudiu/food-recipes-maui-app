using RecipeCabinet.Models;
using System.Diagnostics;
using System.Text.Json;

namespace RecipeCabinet.Services
{
    public class EdamamService : IEdamamService
    {
        private readonly HttpClient _httpClient;
        private readonly string _appId = "EDAMAM_NUTRITION_ANALYSIS_API_APP_ID";
        private readonly string _appKey = "EDAMAM_NUTRITION_ANALYSIS_API_APP_KEY";

        public EdamamService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<NutritionalValues?> GetNutritionDataAsync(string ingredient)
        {
            var url = $"api/nutrition-data?app_id={_appId}&app_key={_appKey}&ingr={Uri.EscapeDataString(ingredient)}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonResult = await response.Content.ReadAsStringAsync();
            Debug.Write(jsonResult);
            try
            {
                var nutritionalValuesObj = JsonSerializer.Deserialize<NutritionalValues>(jsonResult);
                return nutritionalValuesObj;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
