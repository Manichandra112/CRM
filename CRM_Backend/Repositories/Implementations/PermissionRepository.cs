using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly CrmAuthDbContext _context;

    public PermissionRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateAsync(
        string code,
        string description,
        string module)
    {
        if (await _context.Permissions.AnyAsync(p => p.PermissionCode == code))
            throw new Exception("Permission already exists");

        var permission = new Permission
        {
            PermissionCode = code,
            Description = description,
            Module = module,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return permission.PermissionId;
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await _context.Permissions
            .Where(p => p.Active)
            .OrderBy(p => p.PermissionCode)
            .ToListAsync();
    }

    public async Task<long?> GetPermissionIdByCodeAsync(string permissionCode)
    {
        return await _context.Permissions
            .Where(p => p.PermissionCode == permissionCode && p.Active)
            .Select(p => (long?)p.PermissionId)
            .SingleOrDefaultAsync();
    }

    public async Task<List<string>> GetPermissionCodesByIdsAsync(
        IEnumerable<long> permissionIds)
    {
        return await _context.Permissions
            .Where(p => permissionIds.Contains(p.PermissionId) && p.Active)
            .Select(p => p.PermissionCode)
            .OrderBy(code => code)
            .ToListAsync();
    }

    public async Task<Permission?> GetByIdAsync(long id)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.PermissionId == id);
    }

    public async Task UpdateAsync(Permission permission)
    {
        _context.Entry(permission).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
