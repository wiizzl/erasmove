using Erasmove.Views;

namespace Erasmove;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute("MainPage", typeof(MainPage));
    }
}