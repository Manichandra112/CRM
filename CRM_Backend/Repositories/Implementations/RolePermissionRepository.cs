namespace CRM_Backend.Repositories.Implementations;

using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly CrmAuthDbContext _context;

    public RolePermissionRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task AssignPermissionAsync(long roleId, long permissionId, long assignedBy)
    {
        if (await _context.RolePermissions.AnyAsync(rp =>
            rp.RoleId == roleId && rp.PermissionId == permissionId))
            return;

        _context.RolePermissions.Add(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemovePermissionAsync(long roleId, long permissionId)
    {
        var rp = await _context.RolePermissions
            .FirstOrDefaultAsync(x =>
                x.RoleId == roleId && x.PermissionId == permissionId);

        if (rp == null) return;

        _context.RolePermissions.Remove(rp);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<long>> GetPermissionIdsByRoleAsync(long roleId)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();
    }
}
