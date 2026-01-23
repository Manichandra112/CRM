using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly CrmAuthDbContext _context;

    public RoleRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<Role> CreateAsync(
        string roleName,
        string roleCode,
        string? description,
        long domainId,
        bool isSystemRole
    )
    {
        if (await _context.Roles.AnyAsync(r => r.RoleCode == roleCode))
            throw new Exception("Role already exists");

        var role = new Role
        {
            RoleName = roleName,
            RoleCode = roleCode,
            Description = description,
            DomainId = domainId,
            IsSystemRole = isSystemRole,
            Active = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return role;
    }

    // ✅ EXISTING — used by admin role list
    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles
            .Where(r => r.Active)
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    // ✅ EXISTING — used by user creation / role assignment
    public async Task<long> GetRoleIdByCodeAsync(string roleCode)
    {
        return await _context.Roles
            .Where(r => r.RoleCode == roleCode && r.Active)
            .Select(r => r.RoleId)
            .SingleAsync();
    }

    // ✅ NEW — used for domain-scoped dropdowns (Zoho-style)
    public async Task<List<Role>> GetByDomainIdAsync(long domainId)
    {
        return await _context.Roles
            .Where(r =>
                r.DomainId == domainId &&
                r.Active
            )
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }
}
