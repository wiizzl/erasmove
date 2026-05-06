using Erasmove.Models;
using Mapsui.UI.Maui;

namespace Erasmove.Services.Interfaces;

public interface IHomeMapRenderer
{
    void Render(MapControl mapControl, IEnumerable<VoyageEtapeDetail> etapes);
}
