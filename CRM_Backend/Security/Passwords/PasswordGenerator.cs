using System.Security.Cryptography;

namespace CRM_Backend.Security.Passwords;

public static class PasswordGenerator
{
    public static string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(8));
    }
}
