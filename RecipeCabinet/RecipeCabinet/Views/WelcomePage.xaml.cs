using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
		BindingContext = new WelcomePageViewModel();
	}
}