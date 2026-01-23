namespace CRM_Backend.Services.Interfaces;

public interface IPasswordService
{
    string HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string passwordHash);
}
