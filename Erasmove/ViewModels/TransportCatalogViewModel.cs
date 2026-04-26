using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class TransportCatalogViewModel : BaseCatalogViewModel<Transport>
{
    public TransportCatalogViewModel(TransportService service) : base(service, "AddTransport")
    {
    }
}