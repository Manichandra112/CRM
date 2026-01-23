using CRM_Backend.Domain.Entities;


namespace CRM_Backend.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(
        User user,
        bool passwordResetCompleted,
        IEnumerable<string> roles,
        IEnumerable<string> permissions
    );
}
