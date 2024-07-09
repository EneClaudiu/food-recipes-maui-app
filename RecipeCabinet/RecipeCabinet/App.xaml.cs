namespace RecipeCabinet
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Application.Current.UserAppTheme = AppTheme.Light;
            // Dark mode may not display correctly

            MainPage = new AppShell();
        }
    }
}
