using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(SignUpFormViewModel signUpFormViewModel)
	{
		InitializeComponent();
        BindingContext = signUpFormViewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//welcome");
        return true;
    }
}