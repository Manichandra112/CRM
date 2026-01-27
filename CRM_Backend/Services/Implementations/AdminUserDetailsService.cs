using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations;

public class AdminUserDetailsService : IAdminUserDetailsService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserDetailsService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserDetailsDto> GetUserDetailsAsync(long userId)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId && u.DeletedAt == null)
            .Select(u => new AdminUserDetailsDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                AccountStatus = u.AccountStatus,
                LockReason = u.LockReason,

                Profile = new AdminUserProfileDto
                {
                    FirstName = u.Profile.FirstName,
                    LastName = u.Profile.LastName,
                    MobileNumber = u.Profile.MobileNumber,
                    Gender = u.Profile.Gender,
                    LanguagePreference = u.Profile.LanguagePreference,
                    Timezone = u.Profile.Timezone
                },

                Organization = new AdminUserOrganizationDto
                {
                    DomainName = u.Domain.DomainName,
                    Department = u.Department,
                    Designation = u.Designation,
                    ManagerName = u.Manager != null
                        ? u.Manager.Profile.FirstName + " " + u.Manager.Profile.LastName
                        : null,
                    AssignedRegion = u.AssignedRegion,
                    AssignedBranch = u.AssignedBranch,
                    EmploymentType = u.EmploymentType,
                    WorkShift = u.WorkShift
                },

                Roles = u.UserRoles
                    .Select(r => r.Role.RoleName)
                    .ToList(),

                CreatedAt = u.CreatedAt,
                LastActivityAt = u.LastActivityAt
            })
            .FirstOrDefaultAsync();

        return user ?? throw new Exception("User not found");
    }
}
