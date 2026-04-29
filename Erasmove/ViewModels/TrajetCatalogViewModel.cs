using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class TrajetCatalogViewModel : BaseCatalogViewModel<Trajet>
{
    public TrajetCatalogViewModel(ITrajetService service, INavigationService navigationService) : base(service, navigationService, "AddTrajet")
    {
    }
}