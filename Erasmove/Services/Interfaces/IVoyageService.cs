using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface IVoyageService : ICrudService<Voyage>
{
    Task<int> AddVoyageAsync(string libelle, int utilisateurId);
    Task AddVoyageEtapeAsync(int voyageId, int trajetId, int ordre);
}
