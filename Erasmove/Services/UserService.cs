using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Erasmove.Models;

namespace Erasmove.Services;

public class UserService
{
    private const int SaltSize = 32;
    private const int HashSize = 64;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    private const string SessionKey = "current_user_id";
    private readonly string _connectionString;

    public UserService()
    {
        var server = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        _connectionString = $"Server={server},1433;Database=ErasmoveDb;User Id=sa;Password=SuperMotDePasse!123;TrustServerCertificate=True;";
    }

    public async Task InitializeAsync()
    {
        var masterConnectionString = _connectionString.Replace("Database=ErasmoveDb;", "Database=master;");

        await using var masterConnection = new SqlConnection(masterConnectionString);
        await masterConnection.OpenAsync();

        await using var createDbCmd = new SqlCommand(
            "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ErasmoveDb') CREATE DATABASE ErasmoveDb;",
            masterConnection);
        await createDbCmd.ExecuteNonQueryAsync();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var createTableCmd = new SqlCommand("""
            IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
            CREATE TABLE Users (
                Id NVARCHAR(36) PRIMARY KEY,
                Email NVARCHAR(320) NOT NULL UNIQUE,
                PasswordHash NVARCHAR(128) NOT NULL,
                PasswordSalt NVARCHAR(128) NOT NULL,
                FullName NVARCHAR(200) NOT NULL,
                CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
            );
            """, connection);
        await createTableCmd.ExecuteNonQueryAsync();
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = Preferences.Get(SessionKey, string.Empty);
        if (string.IsNullOrEmpty(userId))
            return null;

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            "SELECT Id, Email, FullName FROM Users WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", userId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetString(0),
                Email = reader.GetString(1),
                FullName = reader.GetString(2)
            };
        }

        Preferences.Remove(SessionKey);
        return null;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var trimmedEmail = email.Trim();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            "SELECT Id, Email, FullName, PasswordHash, PasswordSalt FROM Users WHERE Email = @Email", connection);
        command.Parameters.AddWithValue("@Email", trimmedEmail);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var storedHash = reader.GetString(3);
        var storedSalt = reader.GetString(4);

        if (!VerifyPassword(password, storedHash, storedSalt))
            return null;

        var user = new User
        {
            Id = reader.GetString(0),
            Email = reader.GetString(1),
            FullName = reader.GetString(2)
        };

        Preferences.Set(SessionKey, user.Id);
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            "SELECT COUNT(1) FROM Users WHERE Email = @Email", connection);
        command.Parameters.AddWithValue("@Email", email.Trim());

        return (int)await command.ExecuteScalarAsync()! > 0;
    }

    public async Task<User> RegisterAsync(string fullName, string email, string password)
    {
        var salt = GenerateSalt();
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FullName = fullName.Trim(),
            Email = email.Trim()
        };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(
            "INSERT INTO Users (Id, Email, PasswordHash, PasswordSalt, FullName) VALUES (@Id, @Email, @PasswordHash, @PasswordSalt, @FullName)",
            connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", HashPassword(password, salt));
        command.Parameters.AddWithValue("@PasswordSalt", Convert.ToBase64String(salt));
        command.Parameters.AddWithValue("@FullName", user.FullName);

        await command.ExecuteNonQueryAsync();

        Preferences.Set(SessionKey, user.Id);
        return user;
    }

    public void Logout()
    {
        Preferences.Remove(SessionKey);
    }

    private static byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(SaltSize);
    }

    private static string HashPassword(string password, byte[] salt)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, HashSize);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var salt = Convert.FromBase64String(storedSalt);
        var hashToVerify = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, HashSize);
        return CryptographicOperations.FixedTimeEquals(hashToVerify, Convert.FromBase64String(storedHash));
    }
}
