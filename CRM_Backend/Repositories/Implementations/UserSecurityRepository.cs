using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class UserSecurityRepository : IUserSecurityRepository
{
    private readonly CrmAuthDbContext _context;

    public UserSecurityRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    // ---------------- Existing methods (UNCHANGED) ----------------

    public async Task<UserSecurity?> GetByUserIdAsync(long userId)
    {
        return await _context.UserSecurity
            .SingleOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task IncrementFailedAsync(long userId)
    {
        var sec = await GetByUserIdAsync(userId);
        if (sec == null) return;

        sec.FailedLoginCount++;

        if (sec.FailedLoginCount >= 5)
            sec.LockedUntil = DateTime.UtcNow.AddMinutes(15);

        await _context.SaveChangesAsync();
    }

    public async Task ResetFailuresAsync(long userId)
    {
        var sec = await GetByUserIdAsync(userId);
        if (sec == null) return;

        sec.FailedLoginCount = 0;
        sec.LockedUntil = null;
        await _context.SaveChangesAsync();
    }

    public async Task LockUserAsync(long userId, TimeSpan duration)
    {
        var sec = await GetByUserIdAsync(userId);
        if (sec == null) return;

        sec.LockedUntil = DateTime.UtcNow.Add(duration);
        await _context.SaveChangesAsync();
    }

    public async Task ClearForceResetAsync(long userId)
    {
        var security = await _context.UserSecurity
            .FirstAsync(x => x.UserId == userId);

        security.ForcePasswordReset = false;
        security.PasswordLastChangedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    // ---------------- NEW METHODS (Forgot Password) ----------------

    public async Task SetPasswordResetAsync(
        long userId,
        string tokenHash,
        DateTime expiresAt)
    {
        var security = await _context.UserSecurity
            .FirstAsync(x => x.UserId == userId);

        security.PasswordResetTokenHash = tokenHash;
        security.PasswordResetExpiresAt = expiresAt;
        security.ForcePasswordReset = true;

        await _context.SaveChangesAsync();
    }

    public async Task<UserSecurity?> GetByResetTokenHashAsync(
        string tokenHash)
    {
        return await _context.UserSecurity
            .SingleOrDefaultAsync(x =>
                x.PasswordResetTokenHash == tokenHash &&
                x.PasswordResetExpiresAt != null &&
                x.PasswordResetExpiresAt > DateTime.UtcNow
            );
    }

    public async Task ClearPasswordResetAsync(long userId)
    {
        var security = await _context.UserSecurity
            .FirstAsync(x => x.UserId == userId);

        security.PasswordResetTokenHash = null;
        security.PasswordResetExpiresAt = null;
        security.ForcePasswordReset = false;
        security.PasswordLastChangedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
