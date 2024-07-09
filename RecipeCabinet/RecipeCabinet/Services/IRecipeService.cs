using RecipeCabinet.Models;

namespace RecipeCabinet.Services
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<int> AddRecipeAsync(Recipe recipe);
        Task<int> UpdateRecipeAsync(Recipe recipe);
        Task<int> DeleteRecipeAsync(int id);
        Task<bool> UpdateRecipeSyncStatusAsync(int id, bool isSynced);
        Task<List<Recipe>> GetRecipesPagedAsync(int pageNumber, int pageSize);
    }

}
