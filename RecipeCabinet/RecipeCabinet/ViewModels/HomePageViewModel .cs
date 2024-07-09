using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using RecipeCabinet.Models;
using RecipeCabinet.Services;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace RecipeCabinet.ViewModels
{
    public partial class HomePageViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private readonly IRecipeService _recipeService;

        public HomePageViewModel(IRecipeService recipeService)
        {
            this._recipeService = recipeService;
        }

        public ObservableRangeCollection<Recipe> MyRecipes { get; set; } = new();

        private int _pageNumber = 1;
        private const int PAGE_SIZE = 15;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isCreateRecipeOpen = false;

        [ObservableProperty]
        private Recipe newRecipe = new();

        [ObservableProperty]
        private string imageSource = string.Empty;

        [ObservableProperty]
        private bool isPageEmpty = false;

        [RelayCommand]
        private async Task LoadRecipesAsync()
        {
            
            MyRecipes.Clear();
            _pageNumber = 1;
            await LoadMoreRecipesAsync();
        }

        [RelayCommand]
        private async Task LoadMoreRecipesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var recipes = await _recipeService.GetRecipesPagedAsync(_pageNumber, PAGE_SIZE);
                if (recipes != null && recipes.Any())
                {
                    MyRecipes.AddRange(recipes);
                    _pageNumber++;
                    IsPageEmpty = false;
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

            if (MyRecipes.Count == 0) IsPageEmpty = true;
        }

        [RelayCommand]
        private async Task GotoRecipeDetailsAsync(Recipe selectedRecipe)
        {
            var recipeId = selectedRecipe.Id;
            var source = selectedRecipe.Source.ToString();

            var navigationParameter = $"?RecipeId={recipeId}&Source={source}";

            await Shell.Current.GoToAsync($"//recipeDetails{navigationParameter}");

        }

        [RelayCommand]
        private void CreateNewRecipe()
        {
            NewRecipe = new Recipe();
            IsCreateRecipeOpen = true;
            ImageSource = string.Empty;
        }

        [RelayCommand]
        private async Task BrowserImageAsync()
        {
            var image = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select Product Image",
                FileTypes = FilePickerFileType.Images
            });

            if (image == null) return;

            var filePath = Path.Combine(FileSystem.CacheDirectory, image.FileName);
            using (var stream = await image.OpenReadAsync())
            using (var newStream = File.OpenWrite(filePath))
            {
                await stream.CopyToAsync(newStream);
            }

            NewRecipe.ImageUrl = filePath;
            ImageSource = filePath;
        }

        [RelayCommand]
        public async Task SaveRecipeAsync()
        {
            if (string.IsNullOrWhiteSpace(NewRecipe.Name) || string.IsNullOrWhiteSpace(NewRecipe.Description) || NewRecipe == null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }
            try
            {
                NewRecipe.DateCreated = DateTime.Now.Date;
                var result = await _recipeService.AddRecipeAsync(NewRecipe);
                if (result > 0)
                {
                    NewRecipe = new Recipe();
                    IsCreateRecipeOpen = false;
                    await Toast.Make("Recipe saved successfully!", ToastDuration.Short, 15).Show();
                    await LoadRecipesAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to save the recipe.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving recipe: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An error occurred while saving the recipe.", "OK");
            }
        }

        [RelayCommand]
        public void ClosePage()
        {
            IsCreateRecipeOpen = false;
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
