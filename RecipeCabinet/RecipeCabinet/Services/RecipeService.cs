using SQLite;
using RecipeCabinet.Models;

namespace RecipeCabinet.Services
{
    public class RecipeService : IRecipeService
    {
        private SQLiteAsyncConnection _db;

        public RecipeService(IDatabaseService databaseService)
        {
            _db = databaseService.GetLocalConnection();
        }

        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            try
            {
                return await _db.Table<Recipe>().ToListAsync();
            }
            catch (Exception)
            {
                return new List<Recipe>();
            }
        }

        public async Task<List<Recipe>> GetRecipesPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                return await _db.Table<Recipe>()
                               .OrderBy(r => r.Id)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
            }
            catch (Exception)
            {
                return new List<Recipe>();
            }
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _db.Table<Recipe>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> AddRecipeAsync(Recipe recipe)
        {
            return await _db.InsertAsync(recipe);
        }

        public async Task<int> UpdateRecipeAsync(Recipe recipe)
        {
            return await _db.UpdateAsync(recipe);
        }

        public async Task<int> DeleteRecipeAsync(int id)
        {
            return await _db.DeleteAsync<Recipe>(id);
        }

        public async Task<bool> UpdateRecipeSyncStatusAsync(int id, bool isSynced)
        {
            var recipe = await _db.Table<Recipe>().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (recipe != null)
            {
                recipe.IsSynced = isSynced;
                int rowsAffected = await _db.UpdateAsync(recipe);
                return rowsAffected > 0;
            }
            return false;
        }
    }
}
