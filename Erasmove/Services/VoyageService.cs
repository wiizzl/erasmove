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

    public async Task UpdateVoyageAsync(Voyage voyage)
    {
        var parameters = new[]
        {
            new SqlParameter("@VOY_ID", voyage.Id),
            new SqlParameter("@VOY_LIBELLE", voyage.Libelle)
        };

        await Db.ExecuteNonQueryAsync("PSU_VOYAGE", parameters);
    }

    public async Task<int> CreateVoyageWithEtapesAsync(string libelle, int utilisateurId, List<Trajet> itineraireCalcule)
    {
        return await Db.ExecuteInTransactionAsync(async (connection, transaction) =>
        {
            var voyageParams = new[]
            {
                new SqlParameter("@VOY_LIBELLE", libelle),
                new SqlParameter("@UTI_ID", utilisateurId),
                new SqlParameter("@NEW_ID", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            var voyageId = await Db.ExecuteNonQueryAsync("PSI_VOYAGE", connection, transaction, voyageParams, "@NEW_ID");

            for (var i = 0; i < itineraireCalcule.Count; i++)
            {
                var etapeParams = new[]
                {
                    new SqlParameter("@VOY_ID", voyageId),
                    new SqlParameter("@TRJ_ID", itineraireCalcule[i].Id),
                    new SqlParameter("@VET_ORDRE", i + 1)
                };

                await Db.ExecuteNonQueryAsync("PSI_VOYAGE_ETAPE", connection, transaction, etapeParams);
            }

            return voyageId;
        });
    }

    public async Task SaveItineraireCalculeAsync(string libelle, int utilisateurId, List<Trajet> itineraireCalcule)
    {
        await CreateVoyageWithEtapesAsync(libelle, utilisateurId, itineraireCalcule);
    }

    public async Task<List<VoyageEtapeDetail>> GetItineraireVoyageAsync(int voyageId)
    {
        var parameters = new[] { new SqlParameter("@VOY_ID", voyageId) };

        return await Db.ExecuteQueryAsync("PSS_VOYAGE_ITINERARY", reader => new VoyageEtapeDetail
        {
            Ordre = (int)reader["VET_ORDRE"],
            NomDepart = (string)reader["NOM_DEPART"],
            NomArrivee = (string)reader["NOM_ARRIVEE"],
            CompagnieTransport = (string)reader["TRA_COMPAGNIE"],
            TypeTransportLibelle = (string)reader["TYP_LIBELLE"]
        }, parameters);
    }
}