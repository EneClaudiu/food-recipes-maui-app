using RecipeCabinet.Models;

namespace RecipeCabinet.Services
{
    public interface IEdamamService
    {
        Task<NutritionalValues?> GetNutritionDataAsync(string ingredient);
    }
}
