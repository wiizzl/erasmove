using System.Data;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public partial class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService()
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = "localhost",
            InitialCatalog = "erasmove",
            UserID = "sa",
            Password = "SuperMotDePasse!123",
            TrustServerCertificate = true
        };

        _connectionString = connectionStringBuilder.ConnectionString;
    }
    
    public async Task<bool> LoginAsync(string username, string passwordHash)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("sp_LoginUser", conn);
        cmd.CommandType = CommandType.StoredProcedure; 

        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

        SqlParameter successParam = new SqlParameter("@Success", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(successParam);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return successParam.Value != DBNull.Value && (bool)successParam.Value;
    }
    
    /*
    public async Task<List<Station>> GetStationsAsync()
    {
        var stations = new List<Station>();

        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand("sp_GetAllStations", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            stations.Add(new Station
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nom = reader.GetString(reader.GetOrdinal("Nom"))
            });
        }

        return stations;
    }
    */
}