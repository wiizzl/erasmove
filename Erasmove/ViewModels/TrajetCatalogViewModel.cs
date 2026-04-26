using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class TrajetCatalogViewModel : BaseCatalogViewModel<Trajet>
{
    public TrajetCatalogViewModel(TrajetService service) : base(service, "AddTrajet")
    {
    }
}