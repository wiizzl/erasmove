using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface ITrajetService : ICrudService<Trajet>
{
    Task<int> AddTrajetAsync(Trajet trajet);
    Task<List<Trajet>?> FindBestPathAsync(int startLocationId, int endLocationId);
}
