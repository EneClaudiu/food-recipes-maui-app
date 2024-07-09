using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeCabinet.Models;
using RecipeCabinet.Services;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;

namespace RecipeCabinet.ViewModels
{
    public partial class SignUpFormViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        private SignUpFormModel signUpFormModel;

        public SignUpFormViewModel(IApiService apiService)
        {
            _apiService = apiService;
            SignUpFormModel = new SignUpFormModel();
        }

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [RelayCommand]
        private async Task ExecuteSignUpAsync(SfDataForm dataForm)
        {
            if (CanExecuteSignUp(dataForm))
            {
                try
                {
                    await SaveUserAsync();
                    ResetForm();
                    await Shell.Current.GoToAsync("//login");
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

        private bool CanExecuteSignUp(SfDataForm dataForm)
        {
            return dataForm?.Validate() ?? false;
        }

        private async Task SaveUserAsync()
        {
            User user = new User
            {
                Username = SignUpFormModel.Username,
                Email = SignUpFormModel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(SignUpFormModel.Password),
            };
            bool result = await _apiService.RegisterUserAsync(user);
           
            if (!result)
            {
                Debug.WriteLine("User registration failed.");
            }
        }

        private void ResetForm()
        {
            ErrorMessage = string.Empty;
            SignUpFormModel = new SignUpFormModel();
            OnPropertyChanged(nameof(SignUpFormModel));
        }
    }
}
