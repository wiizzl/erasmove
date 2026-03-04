using Erasmove.Services;

namespace Erasmove;

public partial class App : Application
{
    private readonly UserService _userService;

    public App(UserService userService)
    {
        InitializeComponent();
        _userService = userService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell(_userService));
    }
}