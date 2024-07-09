using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _homePageViewModel;

    public HomePage(HomePageViewModel homePageViewModel)
    {
        InitializeComponent();
        BindingContext = homePageViewModel;
        this._homePageViewModel = homePageViewModel;
    }

    protected override void OnAppearing()
    {
        _homePageViewModel.LoadRecipesCommand.Execute(this);
    }
}
