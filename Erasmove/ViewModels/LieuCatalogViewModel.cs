using Erasmove.Models;
using Erasmove.Services;
using Erasmove.Services.Interfaces;
using Erasmove.ViewModels.Base;

namespace Erasmove.ViewModels;

public partial class LieuCatalogViewModel : BaseCatalogViewModel<Lieu>
{
    public LieuCatalogViewModel(ILieuService service, INavigationService navigationService, IStateService stateService) : base(service, navigationService, stateService, "AddLieu")
    {
    }
}