using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace RecipeCabinet.ViewModels
{
    public partial class WelcomePageViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        [RelayCommand]
        private async Task OnContinueAsGuestAsync()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("user_email");
                await Shell.Current.GoToAsync("//homepage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task OnSignInAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//login");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task OnSignUpAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//signup");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation failed: {ex.Message}");
            }
        }
    }
}
