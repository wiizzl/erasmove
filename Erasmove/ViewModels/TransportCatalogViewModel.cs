using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class TransportCatalogViewModel : BaseCatalogViewModel<Transport>
{
    public TransportCatalogViewModel(ITransportService service, INavigationService navigationService, IStateService stateService) : base(service, navigationService, stateService, "AddTransport")
    {
    }
}