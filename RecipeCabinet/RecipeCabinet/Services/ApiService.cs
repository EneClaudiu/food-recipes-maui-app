using RecipeCabinet.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using RecipeCabinet.CustomControls;

namespace RecipeCabinet.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task ApplyAuthorizationHeaderAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting authorization header: {ex.Message}");
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        #region User

        public async Task<bool> RegisterUserAsync(User user)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new HttpRequestException("No internet connection.");
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/users/register", user);
                return response.IsSuccessStatusCode;
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Request timed out: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user: " + ex.Message);
            }
        }
        public async Task<bool> LoginAsync(string email, string password)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new HttpRequestException("No internet connection.");
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/users/login", new User { Email = email, Password = password });
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    await SecureStorage.SetAsync("auth_token", token);
                    await SecureStorage.SetAsync("user_email", email);
                    return true;
                }
                return false;
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Request timed out: " + ex.Message);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Network error: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error connecting to services: " + ex.Message);
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("/api/users/validate-token");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("user_email");
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during logout: {ex.Message}");
            }
        }

        public bool IsUserLoggedIn()
        {
            var token = SecureStorage.GetAsync("auth_token").Result;
            return !string.IsNullOrEmpty(token);
        }

        private async Task<string?> GetUserEmailAsync()
        {
            return await SecureStorage.GetAsync("user_email");
        }

        public async Task<string?> GetUsernameByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"/api/Users/{email}/username");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            await ApplyAuthorizationHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync("/api/users/change-password", changePasswordDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangeUsernameAsync(string changeUsernameRequest)
        {
            await ApplyAuthorizationHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync("/api/users/change-username", changeUsernameRequest);
            return response.IsSuccessStatusCode;
        }
        #endregion User

        #region Recipe
        public async Task<bool> CreateRecipeAsync(Recipe recipe)
        {
            if (!IsUserLoggedIn())
            {
                Debug.WriteLine("User is not logged in.");
                return false;
            }

            await ApplyAuthorizationHeaderAsync();
            await PrepareRecipeForUploadAsync(recipe);

            using (var content = new MultipartFormDataContent())
            {
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    IgnoreReadOnlyProperties = true,
                    Converters = { new DateOnlyJsonConverter() }
                };

                var recipeJson = JsonSerializer.Serialize(recipe, jsonSerializerOptions);
                content.Add(new StringContent(recipeJson, Encoding.UTF8, "application/json"), "recipeJson");

                if (!string.IsNullOrEmpty(recipe.ImageUrl) && File.Exists(recipe.ImageUrl))
                {
                    var imageContent = new StreamContent(File.OpenRead(recipe.ImageUrl));
                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageContent, "imageFile", Path.GetFileName(recipe.ImageUrl));
                }

                try
                {
                    var response = await _httpClient.PostAsync("/api/Recipes", content);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error creating recipe: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<Recipe?> GetRecipeAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Recipes/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Recipe>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving recipe: {ex.Message}");
            }

            return null;
        }

        public async Task<List<Recipe>?> GetAllRecipesAsync()
        {  
            try
            {
                var response = await _httpClient.GetAsync("/api/Recipes");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Recipe>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving recipes: {ex.Message}");
            }

            return new List<Recipe>();
        }

        public async Task<List<Recipe>?> GetRecipesPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Recipes?pageNumber={pageNumber}&pageSize={pageSize}");
                if(response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Recipe>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving recipes: {ex.Message}");
            }
            return new List<Recipe>();
        }

        private async Task PrepareRecipeForUploadAsync(Recipe recipe)
        {
            var userEmail = await GetUserEmailAsync();
            if (userEmail == null)
            {
                throw new Exception("User ID not found.");
            }
            recipe.UserEmail = userEmail;
            recipe.Source = RecipeSource.Remote;
        }

        public async Task<List<Recipe>?> FilterRecipesAsync(string? searchQuery, List<string>? ingredients, float? rating, int? cookTime)
        {
            var queryParameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                queryParameters.Add($"searchQuery={searchQuery}");
            }

            if (ingredients != null && ingredients.Count > 0)
            {
                queryParameters.AddRange(ingredients.Select(ingredient => $"ingredients={ingredient}"));
            }

            if (rating.HasValue)
            {
                queryParameters.Add($"rating={rating}");
            }

            if (cookTime.HasValue)
            {
                queryParameters.Add($"cookTime={cookTime}");
            }

            var queryString = string.Join("&", queryParameters);
            var response = await _httpClient.GetAsync($"/api/Recipes/filter?{queryString}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Recipe>>();
            }

            return new List<Recipe>();
        }

        public async Task<List<Recipe>?> FilterRecipesAsync(string? searchQuery, List<string>? ingredients, float? rating, int? cookTime, int pageNumber, int pageSize, bool randomize)
        {
            var queryParameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                queryParameters.Add($"searchQuery={searchQuery}");
            }

            if (ingredients != null && ingredients.Count > 0)
            {
                queryParameters.AddRange(ingredients.Select(ingredient => $"ingredients={ingredient}"));
            }

            if (rating.HasValue)
            {
                queryParameters.Add($"rating={rating}");
            }

            if (cookTime.HasValue)
            {
                queryParameters.Add($"cookTime={cookTime}");
            }
            queryParameters.Add($"pageNumber={pageNumber}");
            queryParameters.Add($"pageSize={pageSize}");
            queryParameters.Add($"randomize={randomize}");

            var queryString = string.Join("&", queryParameters);
            var response = await _httpClient.GetAsync($"/api/Recipes/filter?{queryString}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Recipe>>();
            }

            return new List<Recipe>();
        }
        #endregion Recipe

        #region RecipeRatings

        public async Task<bool> RateRecipeAsync(RecipeRatings rating)
        {
            var requestContent = new StringContent($"{{\"UserEmail\":\"{rating.UserEmail}\",\"RecipeId\":{rating.RecipeId},\"Rating\":{rating.Rating}}}", System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/RecipeRatings/rate", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<RecipeRatings>?> GetRatingsForRecipeAsync(int recipeId)
        {
            var response = await _httpClient.GetAsync($"/api/RecipeRatings/recipe/{recipeId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<RecipeRatings>>();
            }
            return new List<RecipeRatings>();
        }

        public async Task<List<RecipeRatings>?> GetRatingsByUserAsync(string userEmail)
        {
            var response = await _httpClient.GetAsync($"/api/RecipeRatings/user/{userEmail}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<RecipeRatings>>();
            }
            return new List<RecipeRatings>();
        }

        public async Task<RecipeRatings?> GetRatingAsync(string userEmail, int recipeId)
        {
            var response = await _httpClient.GetAsync($"/api/RecipeRatings/{userEmail}/{recipeId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RecipeRatings>();
            }
            return null;
        }

        #endregion RecipeRatings
    }
}
