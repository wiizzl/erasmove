using Erasmove.Models;

namespace Erasmove.Services.Interfaces;

public interface IVoyageService : ICrudService<Voyage>
{
    Task<int> AddVoyageAsync(string libelle, int utilisateurId);
    Task UpdateVoyageAsync(Voyage voyage);
    Task<int> CreateVoyageWithEtapesAsync(string libelle, int utilisateurId, List<Trajet> itineraireCalcule);
    Task<List<Voyage>> GetVoyagesByUserAsync(int utilisateurId);
    Task<List<VoyageEtapeDetail>> GetItineraireVoyageAsync(int voyageId);
}
