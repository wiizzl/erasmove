using Erasmove.Database;
using Erasmove.Models;
using Microsoft.Data.SqlClient;

namespace Erasmove.Repositories;

public class TransportRepository : BaseCrudRepository<Transport>
{
    public TransportRepository(DatabaseHelper dbHelper) : base(dbHelper)
    {
    }

    protected override string SelectProcedure => "PSS_TRANSPORT";
    protected override string InsertProcedure => "PSI_TRANSPORT";
    protected override string UpdateProcedure => "PSU_TRANSPORT";
    protected override string DeleteProcedure => "PSD_TRANSPORT";

    protected override Transport MapItem(SqlDataReader reader)
    {
        return new Transport
        {
            Id = reader.GetInt32(reader.GetOrdinal("TRP_ID")),
            TransportTypeId = reader.GetInt32(reader.GetOrdinal("TRP_TTY_ID")),
            Company = reader.GetString(reader.GetOrdinal("TRP_COMPANY")),
            Reference = reader.IsDBNull(reader.GetOrdinal("TRP_REFERENCE")) ? string.Empty : reader.GetString(reader.GetOrdinal("TRP_REFERENCE"))
        };
    }

    protected override SqlParameter[] GetInsertParameters(Transport item) =>
    [
        new SqlParameter("@TRP_TTY_ID", item.TransportTypeId),
        new SqlParameter("@TRP_COMPANY", item.Company),
        new SqlParameter("@TRP_REFERENCE", (object)item.Reference ?? DBNull.Value)
    ];

    protected override SqlParameter[] GetUpdateParameters(Transport item) =>
    [
        new SqlParameter("@TRP_ID", item.Id),
        new SqlParameter("@TRP_TTY_ID", item.TransportTypeId),
        new SqlParameter("@TRP_COMPANY", item.Company),
        new SqlParameter("@TRP_REFERENCE", (object)item.Reference ?? DBNull.Value)
    ];

    protected override SqlParameter[] GetDeleteParameters(int id) =>
    [
        new SqlParameter("@TRP_ID", id)
    ];
}