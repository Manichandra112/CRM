using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;

public interface IAdminUserSecurityService
{
    Task<AdminUserSecurityDto> GetUserSecurityAsync(long userId);
}
