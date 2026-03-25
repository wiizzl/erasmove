using Erasmove.Services;
using Erasmove.ViewModels;

namespace Erasmove;

public partial class AppShell : Shell
{
    private readonly AuthService _authService;

    public AppShell(AppShellViewModel viewModel, AuthService authService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _authService = authService;
    }

    protected override async void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        // Si l'utilisateur n'est pas connecté et qu'il essaie d'accéder à la page d'accueil (ou toute autre page qui n'est pas LoginPage)
        if (!_authService.IsAuthenticated && !args.Target.Location.OriginalString.Contains("LoginPage"))
        {
            // Annuler la navigation actuelle et forcer la direction vers LoginPage
            args.Cancel();
            await Current.GoToAsync("//LoginPage");
        }
    }
}