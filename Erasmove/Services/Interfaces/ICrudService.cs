using Erasmove.Models.Interfaces;

namespace Erasmove.Services.Interfaces;

public interface ICrudService<T> where T : IEntity
{
    Task<List<T>> GetAllAsync();
    Task DeleteAsync(int id);
}
