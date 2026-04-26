using Erasmove.Models;
using Erasmove.Services;

namespace Erasmove.ViewModels;

public partial class LieuCatalogViewModel : BaseCatalogViewModel<Lieu>
{
    public LieuCatalogViewModel(LieuService service) : base(service, "AddLieu")
    {
    }
}