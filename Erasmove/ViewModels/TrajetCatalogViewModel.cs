using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class TrajetCatalogViewModel : BaseCatalogViewModel<Trajet>
{
    public TrajetCatalogViewModel(ITrajetService service, INavigationService navigationService, IStateService stateService) : base(service, navigationService, stateService, "AddTrajet")
    {
    }
}