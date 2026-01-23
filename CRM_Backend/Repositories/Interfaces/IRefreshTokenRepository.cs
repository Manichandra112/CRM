using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByHashAsync(string tokenHash);
    Task RevokeAsync(long refreshTokenId);
    Task RevokeAllAsync(long userId);
}
