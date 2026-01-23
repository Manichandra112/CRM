namespace CRM_Backend.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<string>> GetRoleCodesByUserIdAsync(long userId);
    Task<IEnumerable<string>> GetPermissionCodesByUserIdAsync(long userId);
    Task<long> GetRoleIdByCodeAsync(string roleCode);
    Task AssignRoleAsync(long userId, long roleId, long assignedBy);
    Task AssignRoleAsync(long userId, string roleCode, long createdByUserId);
}
