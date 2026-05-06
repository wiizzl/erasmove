using System.Security.Cryptography;
using System.Text;

namespace Erasmove.Helpers;

public static class SecurityHelper
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return string.Empty;
        }

        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash).ToLower();
    }
}