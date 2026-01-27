using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;

public interface IAdminUserListService
{
    Task<AdminUserListResponseDto> GetUsersAsync(
     int page,
     int pageSize);

}
