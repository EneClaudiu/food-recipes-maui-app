using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class AccountPage : ContentPage
{
	private readonly AccountPageViewModel _accountPageViewModel;

	public AccountPage(AccountPageViewModel accountPageViewModel)
	{
		InitializeComponent();
		BindingContext = accountPageViewModel;
		_accountPageViewModel = accountPageViewModel;
	}

    protected override void OnAppearing()
    {
        _accountPageViewModel.LoadConnectedUserCommand.Execute(this);
    }
}