using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using RecipeCabinet.Services;
using RecipeCabinet.ViewModels;
using RecipeCabinet.Views;
using Syncfusion.Maui.Core.Hosting;

namespace RecipeCabinet
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.BaseAddress = new Uri("CUSTOM_API_URI");
                client.Timeout = TimeSpan.FromSeconds(7);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
#if DEBUG
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
#else
            return new HttpClientHandler();
#endif
            });

            builder.Services.AddHttpClient<IEdamamService, EdamamService>("EdamamClien", edamamClient =>
            {
                edamamClient.BaseAddress = new Uri("https://api.edamam.com/");
                edamamClient.Timeout = TimeSpan.FromSeconds(7);
            });

            builder.Services.AddSingleton<IDatabaseService,DatabaseService>();
            builder.Services.AddSingleton<IRecipeService,RecipeService>();

            builder.Services.AddSingleton<HomePageViewModel>();
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddTransient<LoginFormViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<SignUpFormViewModel>();
            builder.Services.AddTransient<SignUpPage>();

            builder.Services.AddTransient<RecipePageViewModel>();
            builder.Services.AddTransient<RecipePage>();

            builder.Services.AddTransient<BrowseRecipesViewModel>();
            builder.Services.AddTransient<BrowseRecipesPage>();

            builder.Services.AddTransient<AccountPageViewModel>();
            builder.Services.AddTransient<AccountPage>();

            builder.Services.AddTransient<NutritionalCalculatorPageViewModel>();
            builder.Services.AddTransient<NutritionalCalculatorPage>();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("SYNCFUSION_LICENSE_KEY");
            builder.ConfigureSyncfusionCore();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
