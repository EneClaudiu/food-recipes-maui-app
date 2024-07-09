using System.Windows.Input;

namespace RecipeCabinet
{
    public partial class AppShell : Shell
    {
        public ICommand LogoutCommand { get; }

        public AppShell()
        {
            InitializeComponent();
            LogoutCommand = new Command<string>(OnLogoutAsync);
            BindingContext = this;
            this.Navigated += HandleNavigated;
            this.Navigating += HandleNavigatingAsync;
        }

        private void HandleNavigated(object sender, ShellNavigatedEventArgs e)
        {
            var route = e.Current.Location.OriginalString;
            if (route.Contains("/homepage") || route.Contains("/browseRecipes") || route.Contains("/recipeDetails") || route.Contains("/account") ||
                route.Contains("/nutritionalCalculator"))
            {
                FlyoutBehavior = FlyoutBehavior.Flyout;
            }
            else
            {
                FlyoutBehavior = FlyoutBehavior.Disabled;
            }
        }

        private async void OnLogoutAsync(string pageName)
        {
            if (pageName == "welcome")
            {
                await SecureStorage.SetAsync("auth_token", string.Empty);
                await SecureStorage.SetAsync("user_email", string.Empty);
                await SecureStorage.SetAsync("saved_email", string.Empty);
                await SecureStorage.SetAsync("saved_auth_token", string.Empty);
                await Shell.Current.GoToAsync("//welcome");
            }
        }

        private async void HandleNavigatingAsync(object sender, ShellNavigatingEventArgs e)
        {
            var authToken = await SecureStorage.GetAsync("auth_token");
            var isLoggedIn = !string.IsNullOrEmpty(authToken);

            var accountItem = this.Items.FirstOrDefault(item => item.Route == "account") as FlyoutItem;
            if (accountItem != null)
            {
                accountItem.IsVisible = isLoggedIn;
            }

            if (!isLoggedIn && e.Target.Location.OriginalString.Contains("account"))
            {
                e.Cancel();
                await Shell.Current.GoToAsync("//login");
            }
        }
    }
}