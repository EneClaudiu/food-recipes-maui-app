using RecipeCabinet.ViewModels;

namespace RecipeCabinet.Views;

public partial class NutritionalCalculatorPage : ContentPage
{
	private readonly NutritionalCalculatorPageViewModel _nutritionalCalculatorPageViewModel;

    public NutritionalCalculatorPage(NutritionalCalculatorPageViewModel nutritionalCalculatorPageViewModel)
	{
		InitializeComponent();
		_nutritionalCalculatorPageViewModel = nutritionalCalculatorPageViewModel;
		BindingContext = _nutritionalCalculatorPageViewModel;
	}

	protected override void OnAppearing()
	{
        _nutritionalCalculatorPageViewModel.LoadRecipesCommand.Execute(this);
    }
}