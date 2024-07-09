using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using RecipeCabinet.Models;
using RecipeCabinet.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace RecipeCabinet.ViewModels
{
    public partial class BrowseRecipesViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private readonly IApiService _apiService;
        public ObservableRangeCollection<Recipe> OnlineRecipes { get; set; } = new();
        public ObservableCollection<string> FilterIngredients { get; set; } = new();

        private int _pageNumber = 1;
        private const int PAGE_SIZE = 10;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private bool isFilterOpen = false;

        [ObservableProperty]
        private string ingredientQuery = string.Empty;

        [ObservableProperty]
        private float ratingFilter = 0;

        [ObservableProperty]
        private int cookTimeFilter;

        [ObservableProperty]
        private bool isErrorVisible;

        public BrowseRecipesViewModel(IApiService apiService)
        {
            this._apiService = apiService;
        }

        [RelayCommand]
        private async Task LoadRecipesAsync()
        {
            try
            {
                IsErrorVisible = false;
                OnlineRecipes.Clear();
                _pageNumber = 1;
                await LoadMoreRecipesAsync();
            }
            catch (Exception)
            {
                IsErrorVisible = true;
            }
        }

        [RelayCommand]
        private async Task LoadMoreRecipesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var recipes = await _apiService.FilterRecipesAsync(
                    string.IsNullOrWhiteSpace(SearchQuery) ? null : SearchQuery,
                    FilterIngredients.ToList(),
                    RatingFilter == 0 ? null : RatingFilter,
                    CookTimeFilter == 0 ? null : CookTimeFilter,
                    _pageNumber,
                    PAGE_SIZE);
                if (recipes != null && recipes.Any())
                {
                    OnlineRecipes.AddRange(recipes);
                    _pageNumber++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading more recipes: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GotoRecipeDetailsAsync(Recipe selectedRecipe)
        {
            var recipeId = selectedRecipe.Id;
            var source = selectedRecipe.Source.ToString();

            var navigationParameter = $"?RecipeId={recipeId}&Source={source}";

            await Shell.Current.GoToAsync($"//recipeDetails{navigationParameter}");
        }

        public async Task SearchBarTextChangedAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await LoadRecipesAsync();
            }
            return;
        }

        [RelayCommand]
        private void OnFilterButtonClicked()
        {
            IsFilterOpen = true;
        }

        [RelayCommand]
        private void AddIngredient()
        {
            if (!string.IsNullOrWhiteSpace(IngredientQuery) && !FilterIngredients.Contains(IngredientQuery))
            {
                FilterIngredients.Add(IngredientQuery);
                IngredientQuery = string.Empty;
            }
        }

        [RelayCommand]
        private void RemoveIngredient(string ingredient)
        {
            if (FilterIngredients.Contains(ingredient))
            {
                FilterIngredients.Remove(ingredient);
            }
        }

        [RelayCommand]
        private async Task ClearFiltersAsync()
        {
            FilterIngredients.Clear();
            IngredientQuery = string.Empty;
            RatingFilter = 0;
            CookTimeFilter = 0;

            await LoadRecipesAsync();
        }

        [RelayCommand]
        private void CloseFilter()
        {
            IsFilterOpen = false;
        }
    }
}
