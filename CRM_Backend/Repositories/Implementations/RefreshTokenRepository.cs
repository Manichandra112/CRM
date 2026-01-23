using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CrmAuthDbContext _context;

    public RefreshTokenRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .Where(t =>
                t.TokenHash == tokenHash &&
                t.RevokedAt == null &&
                t.ExpiresAt > DateTime.UtcNow)
            .SingleOrDefaultAsync();
    }

    public async Task RevokeAsync(long refreshTokenId)
    {
        var token = await _context.RefreshTokens.FindAsync(refreshTokenId);
        if (token == null) return;

        token.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllAsync(long userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
            token.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
