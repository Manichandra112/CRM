using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations;

public class AdminUserAuditLogService : IAdminUserAuditLogService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserAuditLogService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminUserAuditLogDto>> GetUserAuditLogsAsync(long userId)
    {
        return await _context.AuditLogs
            .Where(a => a.TargetUserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AdminUserAuditLogDto
            {
                Action = a.Action,
                Module = a.Module,
                Metadata = a.Metadata,

                ActorName = a.ActorUserId == null
                    ? "SYSTEM"
                    : _context.Users
                        .Where(u => u.UserId == a.ActorUserId)
                        .Select(u => u.Username)
                        .FirstOrDefault(),

                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
    }
}
