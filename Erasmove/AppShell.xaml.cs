using Erasmove.Views;

namespace Erasmove;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute("AddLieu", typeof(AddLieuView));
        Routing.RegisterRoute("AddTransport", typeof(AddTransportView));
        Routing.RegisterRoute("AddTrajet", typeof(AddTrajetView));
        Routing.RegisterRoute("AddUtilisateur", typeof(AddUtilisateurView));
        Routing.RegisterRoute("AddVoyage", typeof(AddVoyageView));
    }
}