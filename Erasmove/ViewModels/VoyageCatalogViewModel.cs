using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class VoyageCatalogViewModel : BaseCatalogViewModel<Voyage>
{
    public VoyageCatalogViewModel(VoyageService service) : base(service, "AddVoyage")
    {
    }
}