using Erasmove.Database;
using Erasmove.Models;
using Microsoft.Data.SqlClient;

namespace Erasmove.Repositories;

public class TripRepository : BaseCrudRepository<Trip>
{
    public TripRepository(DatabaseHelper dbHelper) : base(dbHelper)
    {
    }

    protected override string SelectProcedure => "PSS_TRIP";
    protected override string InsertProcedure => "PSI_TRIP";
    protected override string UpdateProcedure => "PSU_TRIP";
    protected override string DeleteProcedure => "PSD_TRIP";

    protected override Trip MapItem(SqlDataReader reader)
    {
        var trip = new Trip
        {
            Id = reader.GetInt32(reader.GetOrdinal("TRI_ID")),
            DepartureDate = reader.GetDateTime(reader.GetOrdinal("TRI_DEPARTUREDATE")),
            ArrivalDate = reader.GetDateTime(reader.GetOrdinal("TRI_ARRIVALDATE")),
            TravelerId = reader.GetInt32(reader.GetOrdinal("TRI_TRA_ID")),
            PlaceId = reader.GetInt32(reader.GetOrdinal("TRI_PLA_ID")),
            TransportId = reader.GetInt32(reader.GetOrdinal("TRI_TRP_ID"))
        };

        // Essayer de lire les colonnes si elles ont été ajoutées en BD, sans crasher sinon
        try { trip.StartCity = reader["TRI_STARTCITY"] as string ?? ""; } catch { }
        try { trip.EndCity = reader["TRI_ENDCITY"] as string ?? ""; } catch { }
        try { trip.DurationText = reader["TRI_DURATIONTEXT"] as string ?? ""; } catch { }
        try { trip.TripName = reader["TRI_NAME"] as string ?? ""; } catch { }
        try { trip.WaypointsJson = reader["TRI_WAYPOINTS"] as string ?? ""; } catch { }

        return trip;
    }

    protected override SqlParameter[] GetInsertParameters(Trip item) =>
    [
        new("@TRI_DEPARTUREDATE", item.DepartureDate),
        new("@TRI_ARRIVALDATE", item.ArrivalDate),
        new("@TRI_TRA_ID", item.TravelerId),
        new("@TRI_PLA_ID", item.PlaceId),
        new("@TRI_TRP_ID", item.TransportId),
        new("@TRI_STARTCITY", (object)item.StartCity ?? DBNull.Value),
        new("@TRI_ENDCITY", (object)item.EndCity ?? DBNull.Value),
        new("@TRI_DURATIONTEXT", (object)item.DurationText ?? DBNull.Value),
        new("@TRI_NAME", (object)item.TripName ?? DBNull.Value),
        new("@TRI_WAYPOINTS", (object)item.WaypointsJson ?? DBNull.Value)
    ];

    protected override SqlParameter[] GetUpdateParameters(Trip item) =>
    [
        new("@TRI_ID", item.Id),
        new("@TRI_DEPARTUREDATE", item.DepartureDate),
        new("@TRI_ARRIVALDATE", item.ArrivalDate),
        new("@TRI_TRA_ID", item.TravelerId),
        new("@TRI_PLA_ID", item.PlaceId),
        new("@TRI_TRP_ID", item.TransportId),
        new("@TRI_STARTCITY", (object)item.StartCity ?? DBNull.Value),
        new("@TRI_ENDCITY", (object)item.EndCity ?? DBNull.Value),
        new("@TRI_DURATIONTEXT", (object)item.DurationText ?? DBNull.Value),
        new("@TRI_NAME", (object)item.TripName ?? DBNull.Value),
        new("@TRI_WAYPOINTS", (object)item.WaypointsJson ?? DBNull.Value)
    ];

    protected override SqlParameter[] GetDeleteParameters(int id) =>
    [
        new("@TRI_ID", id)
    ];
    
    public async Task<List<Trip>> GetTripsByTravelerIdAsync(int travelerId)
    {
        SqlParameter[] parameters = [ new("@TRI_TRA_ID", travelerId) ];
        return await _dbHelper.ExecuteReaderAsync("PSS_TRIP_BY_TRAVELER", MapItem, parameters);
    }
}