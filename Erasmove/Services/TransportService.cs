using System.Data;
using Erasmove.Models;
using Erasmove.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public class TransportService : BaseCrudService<Transport>, ITransportService
{
    protected override string GetListProcedure => "PSS_TRANSPORT";
    protected override string DeleteProcedure => "PSD_TRANSPORT";
    protected override string IdParameterName => "@TRA_ID";

    public TransportService(DatabaseService db) : base(db) { }

    protected override Transport MapEntity(SqlDataReader reader)
    {
        return new Transport
        {
            Id = (int)reader["TRA_ID"],
            Compagnie = (string)reader["TRA_COMPAGNIE"],
            TypeId = (int)reader["TYP_ID"],
            TypeLibelle = (string)reader["TYP_LIBELLE"]
        };
    }

    public async Task<List<TypeTransport>> GetTypeTransportsAsync()
    {
        return await Db.ExecuteQueryAsync("PSS_TYPE_TRANSPORT", reader => new TypeTransport
        {
            Id = (int)reader["TYP_ID"],
            Libelle = (string)reader["TYP_LIBELLE"]
        });
    }

    public async Task<int> AddTransportAsync(Transport transport)
    {
        var parameters = new[]
        {
            new SqlParameter("@TRA_COMPAGNIE", transport.Compagnie),
            new SqlParameter("@TYP_ID", transport.TypeId),
            new SqlParameter("@NEW_ID", SqlDbType.Int) { Direction = ParameterDirection.Output }
        };

        return await Db.ExecuteNonQueryAsync("PSI_TRANSPORT", parameters, "@NEW_ID");
    }

    public async Task UpdateTransportAsync(Transport transport)
    {
        var parameters = new[]
        {
            new SqlParameter("@TRA_ID", transport.Id),
            new SqlParameter("@TRA_COMPAGNIE", transport.Compagnie),
            new SqlParameter("@TYP_ID", transport.TypeId)
        };

        await Db.ExecuteNonQueryAsync("PSU_TRANSPORT", parameters);
    }
}