using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeCabinet.Models;
using RecipeCabinet.Services;

namespace RecipeCabinet.ViewModels
{
    public partial class AccountPageViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private bool isChangePasswordOpen = false;

        [ObservableProperty]
        private ChangePasswordFormModel changePasswordFormModel;

        [ObservableProperty]
        private string changePasswordErrorMessage = string.Empty;

        [ObservableProperty]
        private bool isChangeUsernameEnabled = false;

        [ObservableProperty]
        private string changeUsernameErrorMessage = string.Empty;

        public AccountPageViewModel(IApiService apiService)
        {
            this._apiService = apiService;
            ChangePasswordFormModel = new ChangePasswordFormModel();
        }

        [RelayCommand]
        public async Task LoadConnectedUserAsync()
        {
            try
            {
                var email = await SecureStorage.GetAsync("user_email");
                Email = email;
                Username = await _apiService.GetUsernameByEmailAsync(Email);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [RelayCommand]
        public async Task ChangePasswordAsync()
        {
            ChangePasswordErrorMessage = string.Empty;

            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = ChangePasswordFormModel.CurrentPassword,
                NewPassword = ChangePasswordFormModel.NewPassword
            };
            var result = await _apiService.ChangePasswordAsync(changePasswordDto);
            if (result)
            {
                await Toast.Make("Password changed successfully!", ToastDuration.Short, 15).Show();
                ClosePopup();
            }
            else
            {
                ChangePasswordErrorMessage = "Failed to change password. Please try again later.";
            }
        }

        [RelayCommand]
        public void ClosePopup()
        {
            IsChangePasswordOpen = false;
        }

        [RelayCommand]
        public async Task OpenChangePasswordAsync()
        {
            IsChangePasswordOpen = true;
            ChangePasswordFormModel = new ChangePasswordFormModel();
            ChangePasswordErrorMessage = string.Empty;

            if (IsChangeUsernameEnabled)
            {
                await DisableChangeUsernameAsync();
            }

        }

        [RelayCommand]
        public void EnableChangeUsername()
        {
            ChangeUsernameErrorMessage = string.Empty;
            IsChangeUsernameEnabled = true;
        }

        [RelayCommand]
        public async Task DisableChangeUsernameAsync()
        {
            IsChangeUsernameEnabled = false;
            Username = await _apiService.GetUsernameByEmailAsync(Email);
            ChangeUsernameErrorMessage = string.Empty;
        }

        [RelayCommand]
        public async Task ChangeUsernameAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ChangeUsernameErrorMessage = "New username cannot be empty.";
                return;
            }

            var result = await _apiService.ChangeUsernameAsync(Username);
            if (result)
            {
                await Toast.Make("Username changed successfully!", ToastDuration.Short, 15).Show();
                await DisableChangeUsernameAsync();
            }
            else
            {
                ChangeUsernameErrorMessage = "Failed to change the username. Please try again.";
            }
        }
    }
}
