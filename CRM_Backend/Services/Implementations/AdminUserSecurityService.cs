using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations;

public class AdminUserSecurityService : IAdminUserSecurityService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserSecurityService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserSecurityDto> GetUserSecurityAsync(long userId)
    {
        var security = await _context.UserSecurity
            .Where(s => s.UserId == userId)
            .Select(s => new AdminUserSecurityDto
            {
                ForcePasswordReset = s.ForcePasswordReset,
                MfaEnabled = s.MfaEnabled,
                MfaType = s.MfaType,
                LastLoginAt = s.LastLoginAt,
                LastLoginIp = s.LastLoginIp,
                LastLoginDevice = s.LastLoginDevice,
                LastLoginLocation = s.LastLoginLocation,
                FailedLoginCount = s.FailedLoginCount,
                LockedUntil = s.LockedUntil,
                PasswordLastChangedAt = s.PasswordLastChangedAt,
                PasswordExpiryDate = s.PasswordExpiryDate
            })
            .FirstOrDefaultAsync();

        return security ?? throw new Exception("User security not found");
    }
}
