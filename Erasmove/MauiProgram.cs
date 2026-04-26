using Erasmove.Services;
using Erasmove.ViewModels;
using Erasmove.Views;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Erasmove;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<AppShell>();
        
        builder.Services.AddSingleton<DatabaseService>();
    
        builder.Services.AddSingleton<LieuService>();
        builder.Services.AddSingleton<TransportService>();
        builder.Services.AddSingleton<UtilisateurService>();
        builder.Services.AddSingleton<TrajetService>();
        builder.Services.AddSingleton<VoyageService>();
        
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginView>();
        
        builder.Services.AddTransient<LieuCatalogViewModel>();
        builder.Services.AddTransient<LieuCatalogView>();
        builder.Services.AddTransient<AddLieuViewModel>();
        builder.Services.AddTransient<AddLieuView>();

        builder.Services.AddTransient<TransportCatalogViewModel>();
        builder.Services.AddTransient<TransportCatalogView>();
        builder.Services.AddTransient<AddTransportViewModel>();
        builder.Services.AddTransient<AddTransportView>();

        builder.Services.AddTransient<UtilisateurCatalogViewModel>();
        builder.Services.AddTransient<UtilisateurCatalogView>();
        builder.Services.AddTransient<AddUtilisateurViewModel>();
        builder.Services.AddTransient<AddUtilisateurView>();

        builder.Services.AddTransient<TrajetCatalogViewModel>();
        builder.Services.AddTransient<TrajetCatalogView>();
        builder.Services.AddTransient<AddTrajetViewModel>();
        builder.Services.AddTransient<AddTrajetView>();

        builder.Services.AddTransient<VoyageCatalogViewModel>();
        builder.Services.AddTransient<VoyageCatalogView>();
        builder.Services.AddTransient<AddVoyageViewModel>();
        builder.Services.AddTransient<AddVoyageView>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}