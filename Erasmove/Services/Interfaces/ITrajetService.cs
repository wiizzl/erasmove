using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface ITrajetService : ICrudService<Trajet>
{
    Task<int> AddTrajetAsync(Trajet trajet);
    Task UpdateTrajetAsync(Trajet trajet);
}
