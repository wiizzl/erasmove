using Erasmove.Services;
using Erasmove.Views;

namespace Erasmove;

public partial class AppShell : Shell
{
    private readonly UserService _userService;
    private bool _initialized;

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

        if (_initialized)
            return;

        _initialized = true;

        await _userService.InitializeAsync();

        if (args.Source == ShellNavigationSource.ShellContentChanged
            && args.Current.Location.OriginalString.Contains("AuthenticationPage")
            && await _userService.GetCurrentUserAsync() is not null)
        {
            await GoToAsync("//MainPage");
        }
    }
}
