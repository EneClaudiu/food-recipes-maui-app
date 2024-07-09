using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginFormViewModel loginFormViewModel)
	{
		InitializeComponent();
        BindingContext = loginFormViewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//welcome");
        return true;
    }
}