using Erasmove.Database;
using Erasmove.Repositories;
using Erasmove.ViewModels;
using Erasmove.Views;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

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
        
        string connectionString = new SqlConnectionStringBuilder()
        {
            DataSource = "localhost",
            InitialCatalog = "CHL_ERASMOVE",
            UserID = "sa",
            Password = "SuperMotDePasse!123",
            TrustServerCertificate = true
        }.ConnectionString;
        
        builder.Services.AddSingleton(new DatabaseHelper(connectionString));

        builder.Services.AddTransient<AccountRepository>();
        builder.Services.AddTransient<PlaceRepository>();
        builder.Services.AddTransient<TransportRepository>();
        builder.Services.AddTransient<TravelerRepository>();
        builder.Services.AddTransient<TripRepository>();

        builder.Services.AddSingleton<AppShell>();
        
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<MainViewModel>();
        
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}