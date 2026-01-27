using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations;

public class AdminUserListService : IAdminUserListService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserListService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserListResponseDto> GetUsersAsync(
     int page,
     int pageSize)
    {
        var query = _context.Users
            .Where(u => u.DeletedAt == null);

        var total = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserListItemDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName,
                Username = u.Username,
                Email = u.Email,
                Department = u.Department,
                Designation = u.Designation,
                AccountStatus = u.AccountStatus,
                ManagerName = u.Manager != null
                    ? u.Manager.Profile.FirstName + " " + u.Manager.Profile.LastName
                    : null,
                Roles = u.UserRoles
                    .Select(r => r.Role.RoleName)
                    .ToList(),
                CreatedAt = u.CreatedAt,
                LastActivityAt = u.LastActivityAt
            })
            .ToListAsync();

        return new AdminUserListResponseDto
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Users = users
        };
    }


}
