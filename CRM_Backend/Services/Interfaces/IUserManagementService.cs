using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;

public interface IUserManagementService
{
 
    Task<long> CreateUserAsync(CreateUserDto dto, long createdBy);
    Task UpdateUserAsync(long userId, UpdateUserDto dto, long updatedBy);

    // -------------------------------

    Task AssignManagerAsync(long userId, long managerId);

    // -------------------------------
    // LOOKUPS / DROPDOWNS
    // -------------------------------
    Task<List<UserLookupDto>> GetUsersAsync();
    Task<List<UserLookupDto>> GetUsersByRoleAsync(string roleCode);
    Task<List<UserLookupDto>> GetEmployeesByDomainAsync(string domainCode);

    // ✅ FIXED: Zoho-style manager dropdown
    Task<List<UserLookupDto>> GetManagersByDomainAsync(string domainCode);

    // -------------------------------
    // USER DETAILS & TEAM
    // -------------------------------
    Task<object> GetUserDetailsAsync(long userId);
    Task<List<UserLookupDto>> GetTeamByManagerAsync(long managerId);

    // -------------------------------
    // ROLES
    // -------------------------------
    Task AssignRoleToUserAsync(long userId, string roleCode, long assignedBy);
    Task RemoveRoleFromUserAsync(long userId, string roleCode);
    Task<List<UserRoleDto>> GetUserRolesAsync(long userId);

    // -------------------------------
    // ACCOUNT STATUS
    // -------------------------------
    Task LockUserAsync(long userId, string reason, long lockedBy);
    Task UnlockUserAsync(long userId, long unlockedBy);

    // -------------------------------
    // ADMIN LIST (Zoho People view)
    // -------------------------------
    Task<List<AdminUserListDto>> GetAdminUsersByDomainAsync(
        string domainCode,
        string? search,
        string? status,
        string? roleCode
    );
}
