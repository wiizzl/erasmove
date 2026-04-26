using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class UtilisateurCatalogViewModel : BaseCatalogViewModel<Utilisateur>
{
    public UtilisateurCatalogViewModel(UtilisateurService service) : base(service, "AddUtilisateur")
    {
    }
}