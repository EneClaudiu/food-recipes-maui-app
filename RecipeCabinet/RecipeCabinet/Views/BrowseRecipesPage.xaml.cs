using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class BrowseRecipesPage : ContentPage
{
	private readonly BrowseRecipesViewModel _browseRecipesViewModel;
    private readonly int _debounceDelay = 1500;
    private CancellationTokenSource _debounceTimer;

    public BrowseRecipesPage(BrowseRecipesViewModel browseRecipesViewModel)
	{
		InitializeComponent();
        BindingContext = browseRecipesViewModel;
        _browseRecipesViewModel = browseRecipesViewModel;
    }

    private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        _debounceTimer?.Cancel();
        _debounceTimer = new CancellationTokenSource();

        try
        {
            await Task.Delay(_debounceDelay, _debounceTimer.Token);

            if (BindingContext is BrowseRecipesViewModel viewModel)
            {
                await viewModel.SearchBarTextChangedAsync();
            }
        }
        catch (TaskCanceledException)
        {}
    }
}