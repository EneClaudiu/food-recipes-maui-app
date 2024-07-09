using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeCabinet.Models;
using RecipeCabinet.Services;
using System.Diagnostics;

namespace RecipeCabinet.ViewModels
{
    [QueryProperty(nameof(RecipeId), "RecipeId")]
    [QueryProperty(nameof(Source), "Source")]
    public partial class RecipePageViewModel : ObservableObject
    {
        private readonly IRecipeService _recipeService;
        private readonly IApiService _apiService;
        private readonly IDispatcher _dispatcher;

        [ObservableProperty]
        private int recipeId;

        [ObservableProperty]
        private string source = string.Empty;

        [ObservableProperty]
        private Recipe selectedRecipe = null!;

        [ObservableProperty]
        private Recipe recipeObject = null!;

        [ObservableProperty]
        private bool isUserLoggedIn = false;

        [ObservableProperty]
        private bool isRecipeLocal = false;

        [ObservableProperty]
        private bool isEditRecipeOpen = false;

        [ObservableProperty]
        private bool isRatingOpen = false;

        [ObservableProperty]
        private string imageSource = string.Empty;

        [ObservableProperty]
        private float userRating;

        [ObservableProperty]
        private string actualSource = string.Empty;

        public RecipePageViewModel(IRecipeService recipeService, IApiService apiService, IDispatcher dispatcher)
        {
            this._recipeService = recipeService;
            this._apiService = apiService;
            this._dispatcher = dispatcher;
        }

        partial void OnRecipeIdChanged(int value) => GetRecipe();
        partial void OnSourceChanged(string value) => GetRecipe();

        private void GetRecipe()
        {
            _dispatcher.Dispatch(async () =>
            {
                if (RecipeId == 0 || string.IsNullOrEmpty(Source)) return;

                if (Source == "Local")
                {
                    SelectedRecipe = await this._recipeService.GetRecipeByIdAsync(RecipeId);
                }
                else
                {
                    SelectedRecipe = await this._apiService.GetRecipeAsync(RecipeId);
                }
                if (SelectedRecipe == null) return;

                await LoadUserRatingAsync();
                IsUserLoggedIn = _apiService.IsUserLoggedIn();
                IsRecipeLocal = Source == "Local";

                if (IsRecipeLocal)
                {
                    ActualSource = "Local";
                }
                else
                {
                    try
                    {
                        ActualSource = await _apiService.GetUsernameByEmailAsync(SelectedRecipe.UserEmail);
                    }
                    catch (Exception)
                    {
                        ActualSource = "Online";
                    }
                }
            });
        }

        private async Task LoadUserRatingAsync()
        {
            var userEmail = await SecureStorage.GetAsync("user_email");
            if (!string.IsNullOrEmpty(userEmail))
            {
                var rating = await _apiService.GetRatingAsync(userEmail, SelectedRecipe.Id);
                if (rating != null)
                {
                    UserRating = rating.Rating;
                }
                else
                {
                    UserRating = 0;
                }
            }
        }

        [RelayCommand]
        private void DownloadRecipe()
        {
            _dispatcher.Dispatch(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert("Download Recipe", "Are you sure you want to download this recipe?", "Yes", "No");
                if (!result) return;

                try
                {
                    Recipe SavedRecipe = SelectedRecipe;
                    SavedRecipe.Id = 0; SavedRecipe.Source = RecipeSource.Local;
                    SavedRecipe.DateCreated = SavedRecipe.DateCreated.Date;

                    if (!string.IsNullOrEmpty(SavedRecipe.ImageUrl))
                    {
                        var localImagePath = await DownloadAndSaveImageAsync(SavedRecipe.ImageUrl);
                        SavedRecipe.ImageUrl = localImagePath;
                    }

                    await this._recipeService.AddRecipeAsync(SelectedRecipe);
                    await Toast.Make("Recipe downloaded successfully!", ToastDuration.Short, 15).Show();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to download the recipe: " + ex.Message);
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to download the recipe.", "OK");
                }
            });
        }

        public async Task<string> DownloadAndSaveImageAsync(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] imageData = await client.GetByteArrayAsync(imageUrl);

                    string fileName = Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                    await File.WriteAllBytesAsync(filePath, imageData);

                    return filePath;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error downloading image: {ex.Message}");
                    throw new Exception("Failed to download image.");
                }
            }
        }

        [RelayCommand]
        private void RateRecipe()
        {
            IsRatingOpen = true;
        }

        [RelayCommand]
        private async Task SaveRatingAsync()
        {
            var userEmail = await SecureStorage.GetAsync("user_email");
            UserRating = (float)(Math.Round(UserRating * 2, MidpointRounding.AwayFromZero) / 2);
            var result = await _apiService.RateRecipeAsync(new RecipeRatings { RecipeId=SelectedRecipe.Id, UserEmail= userEmail, Rating=UserRating});
            if (result)
            {
                await Toast.Make("Rating saved successfully", ToastDuration.Short, 15).Show();
                IsRatingOpen = false;
                GetRecipe();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save the rating.", "OK");
            }
        }

        [RelayCommand]
        private void CancelRating()
        {
            IsRatingOpen = false;
        }

        [RelayCommand]
        private void EditRecipe()
        {
            RecipeObject = SelectedRecipe;
            ImageSource = RecipeObject.ImageUrl;
            IsEditRecipeOpen = true;
        }

        [RelayCommand]
        private void DeleteRecipe()
        {
            _dispatcher.Dispatch(async () =>
            {
                var result = await Application.Current.MainPage.DisplayAlert("Delete Recipe", "Are you sure you want to delete this recipe?", "Yes", "No");
                if (result)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SelectedRecipe.ImageUrl))
                        {
                            var fileExists = File.Exists(SelectedRecipe.ImageUrl);
                            if (fileExists)
                            {
                                File.Delete(SelectedRecipe.ImageUrl);
                            }
                        }

                        var deleteResult = await _recipeService.DeleteRecipeAsync(SelectedRecipe.Id);
                        if (deleteResult > 0)
                        {
                            await Toast.Make("Recipe deleted successfully!", ToastDuration.Short).Show();
                            await Shell.Current.GoToAsync("//homepage");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete the recipe.", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
                    }
                }
            });
        }

        [RelayCommand]
        private void PostRecipe()
        {
            _dispatcher.Dispatch(async () =>
            {
                if (SelectedRecipe.IsSynced == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "This or a version of this recipe is already uploaded.", "OK");
                    return;
                }

                bool confirm = await Application.Current.MainPage.DisplayAlert("Post Recipe", "Are you sure you want to post this recipe?\nOnce uploaded, you may not edit this recipe.", "Yes", "No");
                if (!confirm) return;

                SelectedRecipe.IsSynced = true;

                var result = await _apiService.CreateRecipeAsync(SelectedRecipe);
                if (result)
                {
                    await Toast.Make("Recipe posted successfully!", ToastDuration.Short, 15).Show();
                    await _recipeService.UpdateRecipeSyncStatusAsync(RecipeId, true);
                    await Shell.Current.GoToAsync("//homepage");
                }
                else
                {
                    SelectedRecipe.IsSynced = false;
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to post the recipe.", "OK");
                }
            });
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

            RecipeObject.ImageUrl = filePath;
            ImageSource = filePath;
        }

        [RelayCommand]
        public async Task SaveRecipeAsync()
        {
            if (string.IsNullOrWhiteSpace(RecipeObject.Name) || string.IsNullOrWhiteSpace(RecipeObject.Description) || RecipeObject == null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }
            try
            {
                var result = await _recipeService.UpdateRecipeAsync(RecipeObject);
                if (result > 0)
                {
                    SelectedRecipe = RecipeObject;
                    OnPropertyChanged(nameof(SelectedRecipe));
                    RecipeObject = new Recipe();
                    IsEditRecipeOpen = false;
                    await Toast.Make("Recipe saved successfully!", ToastDuration.Short, 15).Show();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to save the recipe.", "OK");
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "An error occurred while saving the recipe.", "OK");
            }
        }

        [RelayCommand]
        public void ClosePage()
        {
            IsEditRecipeOpen = false;
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
}
