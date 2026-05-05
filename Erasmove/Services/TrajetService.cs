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
            LieuDepartNom = (string)reader["LIE_NOM_DEPART"],
            LieuArriveeNom = (string)reader["LIE_NOM_ARRIVEE"],
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
}