public interface IRolePermissionRepository
{
    Task AssignPermissionAsync(long roleId, long permissionId, long assignedBy);
    Task RemovePermissionAsync(long roleId, long permissionId);

    Task<IEnumerable<long>> GetPermissionIdsByRoleAsync(long roleId);
}
