using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface IVoyageService : ICrudService<Voyage>
{
    Task<int> AddVoyageAsync(string libelle, int utilisateurId);
    Task UpdateVoyageAsync(Voyage voyage);
    Task AddVoyageEtapeAsync(int voyageId, int trajetId, int ordre);
    Task<List<Voyage>> GetVoyagesByUserAsync(int utilisateurId);
    Task<List<VoyageEtapeDetail>> GetItineraireVoyageAsync(int voyageId);
}
