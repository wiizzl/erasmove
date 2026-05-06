using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class VoyageCatalogViewModel : BaseCatalogViewModel<Voyage>
{
    public VoyageCatalogViewModel(IVoyageService service, INavigationService navigationService) : base(service, navigationService, "AddVoyage")
    {
    }
}