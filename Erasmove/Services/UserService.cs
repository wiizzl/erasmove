using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Erasmove.Models;

namespace Erasmove.Services;

public class UserService
{
    private readonly string _filePath;
    private List<User> _users;

    public UserService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");
        _users = LoadUsers();
    }

    public User? GetCurrentUser() => _users.FirstOrDefault(u => u.IsLoggedIn);

    public bool EmailExists(string email)
    {
        return _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public User? Login(string email, string password)
    {
        var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (user is null)
            return null;

        var hashedPassword = HashPassword(password);
        if (user.PasswordHash != hashedPassword)
            return null;

        user.IsLoggedIn = true;
        SaveUsers();
        return user;
    }

    public User Register(string fullName, string email, string password)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FullName = fullName,
            Email = email,
            PasswordHash = HashPassword(password),
            IsLoggedIn = true
        };

        _users.Add(user);
        SaveUsers();
        return user;
    }

    public void Logout()
    {
        foreach (var user in _users)
            user.IsLoggedIn = false;
        SaveUsers();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private List<User> LoadUsers()
    {
        if (!File.Exists(_filePath))
            return [];

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private void SaveUsers()
    {
        var json = JsonSerializer.Serialize(_users);
        File.WriteAllText(_filePath, json);
    }
}
