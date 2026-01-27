using CRM_Backend.DTOs.Permissions;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissions;

    public PermissionService(IPermissionRepository permissions)
    {
        _permissions = permissions;
    }

    public async Task<long> CreateAsync(CreatePermissionDto dto)
    {
        return await _permissions.CreateAsync(
            dto.PermissionCode,
            dto.Description,
            dto.Module
        );
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await _permissions.GetAllAsync();
    }

    // ✅ UPDATE LOGIC
    public async Task UpdateAsync(long id, UpdatePermissionDto dto)
    {
        var permission = await _permissions.GetByIdAsync(id);

        if (permission == null)
            throw new Exception("Permission not found");

        // Business rule:
        // PermissionCode is IMMUTABLE
        permission.Active = dto.IsActive;
        permission.UpdatedAt = DateTime.UtcNow;

        await _permissions.UpdateAsync(permission);
    }
}
