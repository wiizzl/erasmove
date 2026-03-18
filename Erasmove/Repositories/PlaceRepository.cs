using Erasmove.Database;
using Erasmove.Models;
using Microsoft.Data.SqlClient;

namespace Erasmove.Repositories;

public class PlaceRepository : BaseCrudRepository<Place>
{
    public PlaceRepository(DatabaseHelper dbHelper) : base(dbHelper)
    {
    }

    protected override string SelectProcedure => "PSS_PLACE";
    protected override string InsertProcedure => "PSI_PLACE";
    protected override string UpdateProcedure => "PSU_PLACE";
    protected override string DeleteProcedure => "PSD_PLACE";

    protected override Place MapItem(SqlDataReader reader)
    {
        return new Place
        {
            Id = reader.GetInt32(reader.GetOrdinal("PLA_ID")),
            Name = reader.GetString(reader.GetOrdinal("PLA_NAME")),
            City = reader.GetString(reader.GetOrdinal("PLA_CITY")),
            Country = reader.GetString(reader.GetOrdinal("PLA_COUNTRY")),
            Latitude = reader.IsDBNull(reader.GetOrdinal("PLA_LATITUDE")) ? 0 : (double)reader.GetDecimal(reader.GetOrdinal("PLA_LATITUDE")),
            Longitude = reader.IsDBNull(reader.GetOrdinal("PLA_LONGITUDE")) ? 0 : (double)reader.GetDecimal(reader.GetOrdinal("PLA_LONGITUDE"))
        };
    }

    protected override SqlParameter[] GetInsertParameters(Place item) =>
    [
        new SqlParameter("@PLA_NAME", item.Name),
        new SqlParameter("@PLA_CITY", item.City),
        new SqlParameter("@PLA_COUNTRY", item.Country),
        new SqlParameter("@PLA_LATITUDE", item.Latitude),
        new SqlParameter("@PLA_LONGITUDE", item.Longitude)
    ];

    protected override SqlParameter[] GetUpdateParameters(Place item) =>
    [
        new SqlParameter("@PLA_ID", item.Id),
        new SqlParameter("@PLA_NAME", item.Name),
        new SqlParameter("@PLA_CITY", item.City),
        new SqlParameter("@PLA_COUNTRY", item.Country),
        new SqlParameter("@PLA_LATITUDE", item.Latitude),
        new SqlParameter("@PLA_LONGITUDE", item.Longitude)
    ];

    protected override SqlParameter[] GetDeleteParameters(int id) =>
    [
        new SqlParameter("@PLA_ID", id)
    ];
}