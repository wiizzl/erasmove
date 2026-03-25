using Erasmove.Database;
using Erasmove.Models;
using Microsoft.Data.SqlClient;

namespace Erasmove.Repositories;

public class TravelerRepository : BaseCrudRepository<Traveler>
{
    public TravelerRepository(DatabaseHelper dbHelper) : base(dbHelper)
    {
    }

    protected override string SelectProcedure => "PSS_TRAVELER";
    protected override string InsertProcedure => "PSI_TRAVELER";
    protected override string UpdateProcedure => "PSU_TRAVELER";
    protected override string DeleteProcedure => "PSD_TRAVELER";

    protected override Traveler MapItem(SqlDataReader reader)
    {
        return new Traveler
        {
            Id = reader.GetInt32(reader.GetOrdinal("TRA_ID")),
            FirstName = reader.GetString(reader.GetOrdinal("TRA_FIRSTNAME")),
            LastName = reader.GetString(reader.GetOrdinal("TRA_LASTNAME")),
            Phone = reader.IsDBNull(reader.GetOrdinal("TRA_PHONE")) ? string.Empty : reader.GetString(reader.GetOrdinal("TRA_PHONE"))
        };
    }

    protected override SqlParameter[] GetInsertParameters(Traveler item) =>
    [
        new("@TRA_ID", item.Id),
        new("@TRA_FIRSTNAME", item.FirstName),
        new("@TRA_LASTNAME", item.LastName),
        new("@TRA_PHONE", (object)item.Phone ?? DBNull.Value)
    ];

    protected override SqlParameter[] GetUpdateParameters(Traveler item) =>
    [
        new("@TRA_ID", item.Id),
        new("@TRA_FIRSTNAME", item.FirstName),
        new("@TRA_LASTNAME", item.LastName),
        new("@TRA_PHONE", (object)item.Phone ?? DBNull.Value)
    ];

    protected override SqlParameter[] GetDeleteParameters(int id) =>
    [
        new("@TRA_ID", id)
    ];
}