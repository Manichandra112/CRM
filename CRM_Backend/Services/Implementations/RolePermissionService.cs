namespace CRM_Backend.Services.Implementations;

using CRM_Backend.DTOs.Roles;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _repo;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;

    public RolePermissionService(
        IRolePermissionRepository repo,
        IRoleRepository roles,
        IPermissionRepository permissions)
    {
        _repo = repo;
        _roles = roles;
        _permissions = permissions;
    }

    public async Task AssignPermissionAsync(
        string roleCode,
        string permissionCode,
        long adminId)
    {
        var roleId = await _roles.GetRoleIdByCodeAsync(roleCode);
        var permissionId =
            await _permissions.GetPermissionIdByCodeAsync(permissionCode);

        await _repo.AssignPermissionAsync(roleId, permissionId, adminId);
    }

    public async Task RemovePermissionAsync(
        string roleCode,
        string permissionCode)
    {
        var roleId = await _roles.GetRoleIdByCodeAsync(roleCode);
        var permissionId =
            await _permissions.GetPermissionIdByCodeAsync(permissionCode);

        await _repo.RemovePermissionAsync(roleId, permissionId);
    }

    public async Task<IEnumerable<long>> GetPermissionIdsByRoleAsync(long roleId)
    {
        return await _repo.GetPermissionIdsByRoleAsync(roleId);
    }

    public async Task<RolePermissionsResponseDto> GetRolePermissionsAsync(
    string roleCode)
    {
        var roleId = await _roles.GetRoleIdByCodeAsync(roleCode);

        var permissionIds =
            await _repo.GetPermissionIdsByRoleAsync(roleId);

        var permissionCodes =
            await _permissions.GetPermissionCodesByIdsAsync(permissionIds);

        return new RolePermissionsResponseDto
        {
            RoleCode = roleCode,
            Permissions = permissionCodes
        };
    }

}
