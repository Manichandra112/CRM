using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;

public interface IAdminUserDetailsService
{
    Task<AdminUserDetailsDto> GetUserDetailsAsync(long userId);
}
