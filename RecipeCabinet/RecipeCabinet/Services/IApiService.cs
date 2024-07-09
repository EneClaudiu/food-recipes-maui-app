using RecipeCabinet.Models;

namespace RecipeCabinet.Services
{
    public interface IApiService
    {
        Task<bool> RegisterUserAsync(User user);
        Task<bool> LoginAsync(string email, string password);
        void Logout();
        bool IsUserLoggedIn();
        Task<bool> ValidateTokenAsync(string token);
        Task<string?> GetUsernameByEmailAsync(string email);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<bool> ChangeUsernameAsync(string changeUsernameRequest);

        Task<bool> CreateRecipeAsync(Recipe recipe);
        Task<Recipe?> GetRecipeAsync(int id);
        Task<List<Recipe>?> GetAllRecipesAsync();
        Task<List<Recipe>?> GetRecipesPagedAsync(int pageNumber, int pageSize);
        Task<List<Recipe>?> FilterRecipesAsync(string? searchQuery, List<string>? ingredients, float? rating, int? cookTime);
        Task<List<Recipe>?> FilterRecipesAsync(string? searchQuery, List<string>? ingredients, float? rating, int? cookTime, int pageNumber, int pageSize, bool randomize = false);

        Task<bool> RateRecipeAsync(RecipeRatings rating);
        Task<List<RecipeRatings>?> GetRatingsForRecipeAsync(int recipeId);
        Task<List<RecipeRatings>?> GetRatingsByUserAsync(string userEmail);
        Task<RecipeRatings?> GetRatingAsync(string userEmail, int recipeId);
    }
}
