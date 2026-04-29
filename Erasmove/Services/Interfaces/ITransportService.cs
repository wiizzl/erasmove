using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface ITransportService : ICrudService<Transport>
{
    Task<List<TypeTransport>> GetTypeTransportsAsync();
    Task<int> AddTransportAsync(Transport transport);
}
