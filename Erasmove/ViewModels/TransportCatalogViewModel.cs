using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;

namespace Erasmove.ViewModels;

public partial class TransportCatalogViewModel : BaseCatalogViewModel<Transport>
{
    public TransportCatalogViewModel(ITransportService service, INavigationService navigationService) : base(service, navigationService, "AddTransport")
    {
    }
}