using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using RecipeCabinet.Models;
using RecipeCabinet.Services;
using System.Diagnostics;

namespace RecipeCabinet.ViewModels
{
    public partial class NutritionalCalculatorPageViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private readonly IRecipeService _recipeService;
        private readonly IEdamamService _edamamService;

        public NutritionalCalculatorPageViewModel(IRecipeService recipeService, IEdamamService edamamService)
        {
            this._recipeService = recipeService;
            this._edamamService = edamamService;
        }

        public ObservableRangeCollection<Recipe> MyRecipes { get; set; } = new();

        [ObservableProperty]
        private string ingredients = string.Empty;

        [ObservableProperty]
        private Recipe? selectedRecipe;

        [ObservableProperty]
        private bool isResultOpen = false;

        [ObservableProperty]
        private bool isEditorEnabled = true;

        [ObservableProperty]
        private string errorText = string.Empty;

        [ObservableProperty]
        private NutritionalValues nutritionalValuesObj;

        partial void OnSelectedRecipeChanged(Recipe? value)
        {
            if (value != null && !string.IsNullOrWhiteSpace(value.Ingredients))
            {
                Ingredients = value.Ingredients;
            }
            ErrorText = string.Empty;
        }

        [RelayCommand]
        public async Task LoadRecipesAsync()
        {
            ClearEditorCommand.Execute(this);
            try
            {
                MyRecipes.Clear();
                var localRecipes = await _recipeService.GetAllRecipesAsync();
                MyRecipes.AddRange(localRecipes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        public async Task CalculateNutritionAsync()
        {
            ResetEditorFocus();
            if (string.IsNullOrWhiteSpace(Ingredients))
            {
                Debug.WriteLine("No ingredients to analyze.");
                ErrorText = "Please enter ingredients to analyze.";
                return;
            }

            try
            {
                var ingredientsList = Ingredients.Split('\n')
                    .Select(ingredient => ingredient.Trim())
                    .Where(ingredient => !string.IsNullOrEmpty(ingredient))
                    .ToList();

                var allNutritionData = new List<NutritionalValues>();

                foreach (var ingredient in ingredientsList)
                {
                    var nutritionValues = await _edamamService.GetNutritionDataAsync(ingredient);
                    if (nutritionValues != null) allNutritionData.Add(nutritionValues);
                }
                if (allNutritionData.Count == 0)
                {
                    ErrorText = "No valid nutrition data found for the given ingredients.";
                    return;
                }

                NutritionalValuesObj = NutritionalValues.AggregateNutritionData(allNutritionData);
                NutritionalValuesObj.RoundQuantities();

                IsResultOpen = true;
                ErrorText = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error calculating nutrition: {ex.Message}");
                ErrorText = "Error calculating nutrition. Please try again later.";
            }
        }

        [RelayCommand]
        private void CloseResult()
        {
            IsResultOpen = false;
        }

        private void ResetEditorFocus()
        {
            IsEditorEnabled = false;
            IsEditorEnabled = true;
        }

        [RelayCommand]
        public void ClearEditor()
        {
            Ingredients = string.Empty;
            ErrorText = string.Empty;
            SelectedRecipe = null;
        }
    }
}
