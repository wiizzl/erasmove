using Microsoft.Extensions.DependencyInjection;

namespace Erasmove;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var services = activationState?.Context?.Services ?? Current?.Handler?.MauiContext?.Services;
        var shell = services?.GetService<AppShell>();

        return shell is null ? throw new InvalidOperationException("AppShell is not reachable.") : new Window(shell);
    }
}