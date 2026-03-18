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
        var shell = activationState?.Context.Services.GetRequiredService<AppShell>();

        return new Window(shell);
    }
}