using RecipeCabinet.ViewModels;
using System.ComponentModel;

namespace RecipeCabinet.Views;

public partial class RecipePage : ContentPage
{
    private readonly RecipePageViewModel _recipePageViewModel;
    public RecipePage(RecipePageViewModel recipePageViewModel)
    {
        InitializeComponent();
        _recipePageViewModel = recipePageViewModel;
        BindingContext = _recipePageViewModel;

        recipePageViewModel.PropertyChanged += ViewModel_PropertyChanged;
        UpdateToolbarItems(recipePageViewModel);
    }

    protected override bool OnBackButtonPressed()
    {
        if(_recipePageViewModel.IsRecipeLocal)
            Shell.Current.GoToAsync("//homepage");
        else
            Shell.Current.GoToAsync("//browseRecipes");
        return true;
    }

    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is RecipePageViewModel viewModel)
        {
            UpdateToolbarItems(viewModel);
        }
    }
    private void UpdateToolbarItems(RecipePageViewModel viewModel)
    {
        ToolbarItems.Clear();

        if(!viewModel.IsRecipeLocal && viewModel.IsUserLoggedIn)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Rate",
                Command = viewModel.RateRecipeCommand
            });
        }

        if (!viewModel.IsRecipeLocal)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Save",
                Command = viewModel.DownloadRecipeCommand
            });
        }

        if (viewModel.IsRecipeLocal)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Edit",
                Command = viewModel.EditRecipeCommand
            });
        }

        if (viewModel.IsRecipeLocal)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Delete",
                Command = viewModel.DeleteRecipeCommand
            });
        }

        if (viewModel.IsUserLoggedIn && viewModel.IsRecipeLocal)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Post",
                Command = viewModel.PostRecipeCommand
            });
        }
    }
}