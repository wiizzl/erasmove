using Erasmove.Services;
using Erasmove.ViewModels;

namespace Erasmove;

public partial class AppShell : Shell
{
    private readonly AuthService _authService;

    public AppShell(AppShellViewModel viewModel, AuthService authService)
    {
        _authService = authService;
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);

        if (_authService == null) return;

        // Si l'utilisateur n'est pas connecté et qu'il essaie d'accéder à la page d'accueil (ou toute autre page qui n'est pas LoginPage)
        if (!_authService.IsAuthenticated && args.Target != null && !args.Target.Location.OriginalString.Contains("LoginPage"))
        {
            // Annuler la navigation actuelle
            args.Cancel();

            // Effectuer la redirection de manière asynchrone après l'annulation
            Dispatcher.Dispatch(async () =>
            {
                await Current.GoToAsync("//LoginPage");
            });
        }
    }
}