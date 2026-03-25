using System.Data;
using Microsoft.Data.SqlClient;

namespace Erasmove.Database;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<T>> ExecuteReaderAsync<T>(string storedProcedure, Func<SqlDataReader, T> mapItem, SqlParameter[]? parameters = null)
    {
        var list = new List<T>();

        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(storedProcedure, connection);
        command.CommandType = CommandType.StoredProcedure; 

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            list.Add(mapItem(reader));
        }

        return list;
    }

    public async Task<int> ExecuteNonQueryAsync(string storedProcedure, SqlParameter[]? parameters = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(storedProcedure, connection);
        command.CommandType = CommandType.StoredProcedure;

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync();
    }
}