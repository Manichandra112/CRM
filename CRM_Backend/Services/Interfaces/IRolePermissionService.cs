using CRM_Backend.DTOs.Roles;

namespace CRM_Backend.Services.Interfaces;

public interface IRolePermissionService
{
    Task AssignPermissionAsync(
       string roleCode,
       string permissionCode,
       long adminId
   );
    Task RemovePermissionAsync(
       string roleCode,
       string permissionCode
   );
    Task<IEnumerable<long>> GetPermissionIdsByRoleAsync(long roleId);
    Task<RolePermissionsResponseDto> GetRolePermissionsAsync(string roleCode);

}
