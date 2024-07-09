using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeCabinet.Models;
using RecipeCabinet.Services;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;


namespace RecipeCabinet.ViewModels
{
    public partial class LoginFormViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private LoginFormModel loginFormModel;

        public LoginFormViewModel(IApiService apiService)
        {
            _apiService = apiService;
            CheckForSavedCredentialsAsync();
            LoginFormModel = new LoginFormModel();
        }

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isRememberMeChecked;

        [RelayCommand]
        private async Task ExecuteLoginAsync(SfDataForm dataForm)
        {
            if (CanExecuteLogin(dataForm))
            {
                try
                {
                    var token = await _apiService.LoginAsync(LoginFormModel.Email, LoginFormModel.Password);

                    if (token)
                    {
                        if(IsRememberMeChecked)
                        {
                            await SecureStorage.SetAsync("saved_email", LoginFormModel.Email);
                            await SecureStorage.SetAsync("saved_auth_token", await SecureStorage.GetAsync("auth_token"));
                        }
                        else
                        {
                            await SecureStorage.SetAsync("saved_email", string.Empty);
                            await SecureStorage.SetAsync("saved_auth_token", string.Empty);
                        }
                        ResetForm();
                        await Shell.Current.GoToAsync("//homepage");
                        await ShowLoggedInUserToastAsync();
                    }
                    else
                    {
                        ErrorMessage = "Invalid credentials.";
                    }
                }
                catch (HttpRequestException)
                {
                    ErrorMessage = "Error connecting to online services.";
                }
                catch (TaskCanceledException)
                {
                    ErrorMessage = "Request timed out.";
                }
                catch (Exception)
                {
                    ErrorMessage = "An unexpected error occurred. Please try again later.";
                }
            }
        }

        private bool CanExecuteLogin(SfDataForm dataForm)
        {
            return dataForm?.Validate() ?? false;
        }

        [RelayCommand]
        private async Task ExecuteSignUpAsync()
        {
            try
            {
                ResetForm();
                await Shell.Current.GoToAsync("//signup");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        private void ResetForm()
        {
            ErrorMessage = string.Empty;
            LoginFormModel = new LoginFormModel();
            OnPropertyChanged(nameof(LoginFormModel));
        }

        private async void CheckForSavedCredentialsAsync()
        {
            var savedEmail = await SecureStorage.GetAsync("saved_email");
            var savedToken = await SecureStorage.GetAsync("saved_auth_token");

            if (!string.IsNullOrEmpty(savedEmail) && !string.IsNullOrEmpty(savedToken))
            {
                var isValidToken = await _apiService.ValidateTokenAsync(savedToken);
                if (isValidToken)
                {
                    await SecureStorage.SetAsync("user_email", savedEmail);
                    await SecureStorage.SetAsync("auth_token", savedToken);
                    ResetForm();
                    await Shell.Current.GoToAsync("//homepage");
                    await ShowLoggedInUserToastAsync();
                }
            }
        }

        private async Task ShowLoggedInUserToastAsync()
        {
            var userEmail = await SecureStorage.GetAsync("user_email");
            if (!string.IsNullOrEmpty(userEmail))
            {
                await Toast.Make($"Logged in with {userEmail}", ToastDuration.Short, 15).Show();
            }
        }
    }
}
