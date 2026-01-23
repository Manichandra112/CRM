using BCrypt.Net;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations;

public class PasswordService : IPasswordService
{
    public string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty.");

        // BCrypt automatically handles salt
        return BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return false;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
    }
}
