using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface ILieuService : ICrudService<Lieu>
{
    Task<int> AddLieuAsync(Lieu lieu);
    Task UpdateLieuAsync(Lieu lieu);
}
