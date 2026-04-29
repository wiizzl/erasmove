using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class UtilisateurCatalogViewModel : BaseCatalogViewModel<Utilisateur>
{
    public UtilisateurCatalogViewModel(IUtilisateurService service, INavigationService navigationService) : base(service, navigationService, "AddUtilisateur")
    {
    }
}