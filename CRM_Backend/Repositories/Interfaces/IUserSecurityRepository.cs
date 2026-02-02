using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IUserSecurityRepository
{
    Task<UserSecurity?> GetByUserIdAsync(long userId);
    Task IncrementFailedAsync(long userId);
    Task ResetFailuresAsync(long userId);
    Task LockUserAsync(long userId, TimeSpan duration);
    Task ClearForceResetAsync(long userId);


    Task SetPasswordResetAsync(
        long userId,
        string tokenHash,
        DateTime expiresAt
    );

    Task<UserSecurity?> GetByResetTokenHashAsync(
        string tokenHash
    );

    Task ClearPasswordResetAsync(
        long userId
    );

    Task UpdateLastLoginAsync(
    long userId,
    string ipAddress,
    string userAgent,
    string? location = null
);
}
