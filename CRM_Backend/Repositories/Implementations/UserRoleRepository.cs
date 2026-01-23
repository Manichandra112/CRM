using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly CrmAuthDbContext _context;

    public UserRoleRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetRoleCodesByUserIdAsync(long userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_context.Roles,
                  ur => ur.RoleId,
                  r => r.RoleId,
                  (ur, r) => r.RoleCode)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetPermissionCodesByUserIdAsync(long userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_context.RolePermissions,
                  ur => ur.RoleId,
                  rp => rp.RoleId,
                  (ur, rp) => rp.PermissionId)
            .Join(_context.Permissions,
                  pid => pid,
                  p => p.PermissionId,
                  (pid, p) => p.PermissionCode)
            .Distinct()
            .ToListAsync();
    }

    public async Task<long> GetRoleIdByCodeAsync(string roleCode)
    {
        return await _context.Roles
            .Where(r => r.RoleCode == roleCode && r.Active)
            .Select(r => r.RoleId)
            .SingleAsync();
    }

    // Assign by RoleId
    public async Task AssignRoleAsync(long userId, long roleId, long assignedBy)
    {
        var exists = await _context.UserRoles
            .AnyAsync(x => x.UserId == userId && x.RoleId == roleId);

        if (exists)
            return;

        _context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    // Assign by RoleCode
    public async Task AssignRoleAsync(long userId, string roleCode, long createdByUserId)
    {
        var roleId = await GetRoleIdByCodeAsync(roleCode);
        await AssignRoleAsync(userId, roleId, createdByUserId);
    }
}
