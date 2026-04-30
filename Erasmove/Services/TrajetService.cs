using System.Data;
using Erasmove.Models;
using Erasmove.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public class TrajetService : BaseCrudService<Trajet>, ITrajetService
{
    protected override string GetListProcedure => "PSS_TRAJET";
    protected override string DeleteProcedure => "PSD_TRAJET";
    protected override string IdParameterName => "@TRJ_ID";

    public TrajetService(DatabaseService db) : base(db) { }

    protected override Trajet MapEntity(SqlDataReader reader)
    {
        return new Trajet
        {
            Id = (int)reader["TRJ_ID"],
            LieuDepartId = (int)reader["LIE_ID_DEPART"],
            LieuArriveeId = (int)reader["LIE_ID_ARRIVEE"],
            TransportId = (int)reader["TRA_ID"],
            NomDepart = (string)reader["NOM_DEPART"],
            VilleDepart = (string)reader["VILLE_DEPART"],
            PaysDepart = (string)reader["PAYS_DEPART"],
            NomArrivee = (string)reader["NOM_ARRIVEE"],
            VilleArrivee = (string)reader["VILLE_ARRIVEE"],
            PaysArrivee = (string)reader["PAYS_ARRIVEE"],
            CompagnieTransport = (string)reader["TRA_COMPAGNIE"],
            TypeTransportLibelle = (string)reader["TYP_LIBELLE"]
        };
    }

    public async Task<int> AddTrajetAsync(Trajet trajet)
    {
        var parameters = new[]
        {
            new SqlParameter("@LIE_ID_DEPART", trajet.LieuDepartId),
            new SqlParameter("@LIE_ID_ARRIVEE", trajet.LieuArriveeId),
            new SqlParameter("@TRA_ID", trajet.TransportId),
            new SqlParameter("@NEW_ID", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        return await Db.ExecuteNonQueryAsync("PSI_TRAJET", parameters, "@NEW_ID");
    }

    public async Task UpdateTrajetAsync(Trajet trajet)
    {
        var parameters = new[]
        {
            new SqlParameter("@TRJ_ID", trajet.Id),
            new SqlParameter("@LIE_ID_DEPART", trajet.LieuDepartId),
            new SqlParameter("@LIE_ID_ARRIVEE", trajet.LieuArriveeId),
            new SqlParameter("@TRA_ID", trajet.TransportId)
        };

        await Db.ExecuteNonQueryAsync("PSU_TRAJET", parameters);
    }

    public async Task<List<Trajet>?> FindBestPathAsync(int startLocationId, int endLocationId)
    {
        var allSegments = await GetAllAsync();
        var pathQueue = new Queue<List<Trajet>>();

        var startingPoints = allSegments.Where(t => t.LieuDepartId == startLocationId);
        foreach (var start in startingPoints)
        {
            pathQueue.Enqueue([start]);
        }

        while (pathQueue.Count > 0)
        {
            var currentPath = pathQueue.Dequeue();
            var lastSegment = currentPath.Last();

            if (lastSegment.LieuArriveeId == endLocationId)
            {
                return currentPath;
            }

            var nextSteps = allSegments.Where(t =>
                t.LieuDepartId == lastSegment.LieuArriveeId &&
                currentPath.All(p => p.LieuDepartId != t.LieuArriveeId));

            foreach (var step in nextSteps)
            {
                var newPath = new List<Trajet>(currentPath) { step };
                pathQueue.Enqueue(newPath);
            }
        }

        return null;
    }
}