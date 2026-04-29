using Erasmove.Models;
using Erasmove.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public class LieuService : BaseCrudService<Lieu>, ILieuService
{
    protected override string GetListProcedure => "PSS_LIEU";
    protected override string DeleteProcedure => "PSD_LIEU";
    protected override string IdParameterName => "@LIE_ID";

    public LieuService(DatabaseService db) : base(db) { }

    protected override Lieu MapEntity(SqlDataReader reader)
    {
        return new Lieu
        {
            Id = (int)reader["LIE_ID"],
            Nom = (string)reader["LIE_NOM"],
            Ville = (string)reader["LIE_VILLE"],
            Pays = (string)reader["LIE_PAYS"],
            Latitude = (double)reader["LIE_LATITUDE"],
            Longitude = (double)reader["LIE_LONGITUDE"]
        };
    }

    public async Task<int> AddLieuAsync(Lieu lieu)
    {
        var parameters = new[]
        {
            new SqlParameter("@LIE_NOM", lieu.Nom),
            new SqlParameter("@LIE_VILLE", lieu.Ville),
            new SqlParameter("@LIE_PAYS", lieu.Pays),
            new SqlParameter("@LIE_LATITUDE", lieu.Latitude),
            new SqlParameter("@LIE_LONGITUDE", lieu.Longitude),
            new SqlParameter("@NEW_ID", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output }
        };

        return await Db.ExecuteNonQueryAsync("PSI_LIEU", parameters, "@NEW_ID");
    }
}