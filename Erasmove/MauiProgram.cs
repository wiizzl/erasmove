using Microsoft.Extensions.Logging;
using Erasmove.Services;
using Erasmove.ViewModels;
using Erasmove.Views;

namespace Erasmove;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register Services
        builder.Services.AddSingleton<UserService>();

        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<AuthenticationViewModel>();
        builder.Services.AddTransient<AccountViewModel>();

        // Register Views
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<AuthenticationPage>();
        builder.Services.AddTransient<AccountPage>();

        return builder.Build();
    }
}