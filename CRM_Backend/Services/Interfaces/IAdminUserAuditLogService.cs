using CRM_Backend.DTOs.Users;

namespace CRM_Backend.Services.Interfaces;

public interface IAdminUserAuditLogService
{
    Task<List<AdminUserAuditLogDto>> GetUserAuditLogsAsync(long userId);
}
