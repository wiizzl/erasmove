using System.Data;
using Erasmove.Models;
using Erasmove.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public class VoyageService : BaseCrudService<Voyage>, IVoyageService
{
    protected override string GetListProcedure => "PSS_VOYAGE";
    protected override string DeleteProcedure => "PSD_VOYAGE";
    protected override string IdParameterName => "@VOY_ID";

    public VoyageService(DatabaseService db) : base(db) { }

    protected override Voyage MapEntity(SqlDataReader reader)
    {
        return new Voyage
        {
            Id = (int)reader["VOY_ID"],
            Libelle = (string)reader["VOY_LIBELLE"],
            UtilisateurId = (int)reader["UTI_ID"],
            DateCreation = (DateTime)reader["VOY_DATE_CREATION"],
            UtilisateurNom = (string)reader["UTI_NOM"],
            UtilisateurPrenom = (string)reader["UTI_PRENOM"]
        };
    }

    public async Task<int> AddVoyageAsync(string libelle, int utilisateurId)
    {
        var parameters = new[]
        {
            new SqlParameter("@VOY_LIBELLE", libelle),
            new SqlParameter("@UTI_ID", utilisateurId),
            new SqlParameter("@NEW_ID", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        return await Db.ExecuteNonQueryAsync("PSI_VOYAGE", parameters, "@NEW_ID");
    }

    public async Task AddVoyageEtapeAsync(int voyageId, int trajetId, int ordre)
    {
        var parameters = new[]
        {
            new SqlParameter("@VOY_ID", voyageId),
            new SqlParameter("@TRJ_ID", trajetId),
            new SqlParameter("@VET_ORDRE", ordre)
        };

        await Db.ExecuteNonQueryAsync("PSI_VOYAGE_ETAPE", parameters);
    }

    public async Task SaveItineraireCalculeAsync(string libelle, int utilisateurId, List<Trajet> itineraireCalcule)
    {
        var newVoyageId = await AddVoyageAsync(libelle, utilisateurId);

        var actualOrder = 1;
        foreach (var etape in itineraireCalcule)
        {
            await AddVoyageEtapeAsync(newVoyageId, etape.Id, actualOrder);
            actualOrder++;
        }
    }

    public async Task<List<VoyageEtapeDetail>> GetItineraireVoyageAsync(int voyageId)
    {
        var parameters = new[] { new SqlParameter("@VOY_ID", voyageId) };

        return await Db.ExecuteQueryAsync("PSS_VOYAGE_ITINERARY", reader => new VoyageEtapeDetail
        {
            Ordre = (int)reader["VET_ORDRE"],
            VilleDepart = (string)reader["VILLE_DEPART"],
            VilleArrivee = (string)reader["VILLE_ARRIVEE"],
            CompagnieTransport = (string)reader["TRA_COMPAGNIE"],
            TypeTransportLibelle = (string)reader["TYP_LIBELLE"]
        }, parameters);
    }
}