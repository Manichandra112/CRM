using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;

public interface IUserManagementService
{
    Task<long> CreateUserAsync(CreateUserDto dto, long createdBy);
    Task AssignManagerAsync(long userId, long managerId);
    Task<List<UserLookupDto>> GetUsersByRoleAsync(string roleCode);
    Task<List<UserLookupDto>> GetUsersAsync();

    Task<List<UserLookupDto>> GetEmployeesByDomainAsync(string domainCode);
    Task<List<UserLookupDto>> GetManagersByDomainAsync(
      string domainCode,
      string roleCode
  );
    Task<object> GetUserDetailsAsync(long userId);
    Task<List<UserLookupDto>> GetTeamByManagerAsync(long managerId);


    Task AssignRoleToUserAsync(long userId, string roleCode, long assignedBy);
    Task RemoveRoleFromUserAsync(long userId, string roleCode);
    Task<List<UserRoleDto>> GetUserRolesAsync(long userId);

    Task UpdateUserAsync(long userId, UpdateUserDto dto, long updatedBy);

    Task LockUserAsync(long userId, string reason, long lockedBy);
    Task UnlockUserAsync(long userId, long unlockedBy);


    Task<List<AdminUserListDto>> GetAdminUsersByDomainAsync(
    string domainCode,
    string? search,
    string? status,
    string? roleCode
);



}
