using System.Data;
using Microsoft.Data.SqlClient;

namespace Erasmove.Services;

public class DatabaseService
{
    private static string GetConnectionString()
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = "localhost,1433",
            InitialCatalog = "CHL_ERASMOVE",
            UserID = "sa",
            Password = "SuperMotDePasse!123",
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(string procedureName, Func<SqlDataReader, T> mapFunction, SqlParameter[]? parameters = null)
    {
        var results = new List<T>();

        await using var connection = new SqlConnection(GetConnectionString());
        await using var command = new SqlCommand(procedureName, connection);
        
        command.CommandType = CommandType.StoredProcedure;
        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            results.Add(mapFunction(reader));
        }

        return results;
    }
    
    public async Task<int> ExecuteNonQueryAsync(string procedureName, SqlParameter[]? parameters = null, string? outputIdParam = null)
    {
        await using var connection = new SqlConnection(GetConnectionString());
        await using var command = new SqlCommand(procedureName, connection);
        
        command.CommandType = CommandType.StoredProcedure;
        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        
        if (!string.IsNullOrEmpty(outputIdParam) && command.Parameters.Contains(outputIdParam))
        {
            return (int)command.Parameters[outputIdParam].Value;
        }

        return 0;
    }
}