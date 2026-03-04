using Erasmove.Services;
using Erasmove.Views;

namespace Erasmove;

public partial class AppShell : Shell
{
    private readonly UserService _userService;

    public AppShell(UserService userService)
    {
        InitializeComponent();
        _userService = userService;

        Routing.RegisterRoute(nameof(AuthenticationPage), typeof(AuthenticationPage));
        Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        // On first navigation, redirect to MainPage if already logged in
        if (args.Source == ShellNavigationSource.ShellContentChanged
            && args.Current.Location.OriginalString.Contains("AuthenticationPage")
            && _userService.GetCurrentUser() is not null)
        {
            await GoToAsync("//MainPage");
        }
    }
}
