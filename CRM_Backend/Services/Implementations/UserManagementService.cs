using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace CRM_Backend.Services.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly CrmAuthDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IRoleRepository _roles;
    private readonly IDomainRepository _domains;
    private readonly IUserRepository _users;
    private readonly IAuditLogService _audit;

    public UserManagementService(
        CrmAuthDbContext context,
        IPasswordService passwordService,
        IRoleRepository roles,
        IDomainRepository domains,
        IUserRepository users,
        IAuditLogService audit)
    {
        _context = context;
        _passwordService = passwordService;
        _roles = roles;
        _domains = domains;
        _users = users;
        _audit = audit;
    }

    public async Task<long> CreateUserAsync(CreateUserDto dto, long createdBy)
    {
        // 1️⃣ Domain validation
        var domain = await _domains.GetByCodeAsync(dto.DomainCode)
            ?? throw new Exception("Invalid domain");

        // 2️⃣ Uniqueness check
        var userExists = await _context.Users.AnyAsync(u =>
            u.Email == dto.Email || u.Username == dto.Username);

        if (userExists)
            throw new Exception("User already exists");

        // 3️⃣ User
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Department = dto.Profile.Department,
            Designation = dto.Profile.Designation,
            AccountStatus = "ACTIVE",
            DomainId = domain.DomainId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedVia = "ADMIN"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 4️⃣ Profile
        _context.UserProfiles.Add(new UserProfile
        {
            UserId = user.UserId,
            FirstName = dto.Profile.FirstName,
            LastName = dto.Profile.LastName,
            MobileNumber = dto.Profile.MobileNumber
        });

        // 5️⃣ Security
        _context.UserSecurity.Add(new UserSecurity
        {
            UserId = user.UserId,
            ForcePasswordReset = true,
            FailedLoginCount = 0,
            MfaEnabled = false
        });

        // 6️⃣ Password
        _context.UserPasswords.Add(new UserPassword
        {
            UserId = user.UserId,
            PasswordHash = _passwordService.HashPassword(dto.TemporaryPassword),
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow
        });

        // 7️⃣ Roles
        foreach (var roleCode in dto.RoleCodes)
        {
            var roleId = await _roles.GetRoleIdByCodeAsync(roleCode);

            _context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = createdBy
            });
        }

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = createdBy,
            TargetUserId = user.UserId,
            Action = "USER_CREATE",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                domain = dto.DomainCode
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return user.UserId;
    }

    // ✅ SINGLE, CORRECT METHOD — NO DUPLICATES
    public async Task AssignManagerAsync(long userId, long managerId)
    {
        if (userId == managerId)
            throw new Exception("User cannot be their own manager");

        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        var manager = await _context.Users.FindAsync(managerId)
            ?? throw new Exception("Manager not found");

        user.ManagerId = managerId;
        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = managerId,
            TargetUserId = userId,
            Action = "ASSIGN_MANAGER",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                managerId = managerId
            }),

            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task<List<UserLookupDto>> GetUsersByRoleAsync(string roleCode)
    {
        return await _context.UserRoles
            .Where(ur => ur.Role.RoleCode == roleCode)
            .Select(ur => new UserLookupDto
            {
                UserId = ur.UserId,
                Name = ur.User.Profile.FirstName + " " + ur.User.Profile.LastName
            })
            .ToListAsync();
    }

    public async Task<List<UserLookupDto>> GetUsersAsync()
    {
        return await _context.Users
            .Where(u => u.AccountStatus == "ACTIVE")
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }
    // --------------------------------------------------
    // 🔹 GET EMPLOYEES BY DOMAIN
    // --------------------------------------------------
    public async Task<List<UserLookupDto>> GetEmployeesByDomainAsync(string domainCode)
    {
        return await _context.Users
            .Where(u =>
                u.AccountStatus == "ACTIVE" &&
                u.Domain.DomainCode == domainCode
            )
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .ToListAsync();
    }

    // --------------------------------------------------
    // 🔹 GET MANAGERS BY DOMAIN (permission-based)
    // --------------------------------------------------
    public async Task<List<UserLookupDto>> GetManagersByDomainAsync(string domainCode)
    {
        var domain = await _domains.GetByCodeAsync(domainCode)
            ?? throw new Exception("Invalid domain");

        return await _context.Users
            .Where(u =>
                u.AccountStatus == "ACTIVE" &&
                u.DomainId == domain.DomainId &&
                u.UserRoles.Any(ur => ur.Role.RoleCode.EndsWith("_MANAGER"))
            )
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }



    // --------------------------------------------------
    // 🔹 GET SINGLE USER DETAILS (View/Edit)
    // --------------------------------------------------
    public async Task<object> GetUserDetailsAsync(long userId)
    {
        var user = await _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new
            {
                u.UserId,
                u.Username,
                u.Email,
                u.Department,
                u.Designation,
                u.AccountStatus,

                ManagerId = u.ManagerId,

                ManagerName = u.Manager != null
                    ? u.Manager.Profile.FirstName + " " + u.Manager.Profile.LastName
                    : null,

                Profile = new
                {
                    u.Profile.FirstName,
                    u.Profile.LastName,
                    u.Profile.MobileNumber
                }
            })
            .FirstOrDefaultAsync();

        return user ?? throw new Exception("User not found");
    }


    // --------------------------------------------------
    // 🔹 GET TEAM BY MANAGER (Zoho “My Team”)
    // --------------------------------------------------
    public async Task<List<UserLookupDto>> GetTeamByManagerAsync(long managerId)
    {
        return await _context.Users
            .Where(u => u.ManagerId == managerId)
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName
            })
            .ToListAsync();
    }

    public async Task AssignRoleToUserAsync(long userId, string roleCode, long assignedBy)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleCode == roleCode)
            ?? throw new Exception("Role not found");

        var alreadyAssigned = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);

        if (alreadyAssigned)
            throw new Exception("Role already assigned to user");

        _context.UserRoles.Add(new UserRole
        {
            UserId = userId,
            RoleId = role.RoleId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = assignedBy
        });

        await _context.SaveChangesAsync();
    }
    public async Task RemoveRoleFromUserAsync(long userId, string roleCode)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleCode == roleCode)
            ?? throw new Exception("Role not found");

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId)
            ?? throw new Exception("Role not assigned to user");

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserRoleDto>> GetUserRolesAsync(long userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => new UserRoleDto
            {
                RoleCode = ur.Role.RoleCode,
                RoleName = ur.Role.RoleName
            })
            .ToListAsync();
    }
    public async Task UpdateUserAsync(long userId, UpdateUserDto dto, long updatedBy)
    {
        var user = await _context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new Exception("User not found");

        // Track what changed (important for audit)
        var changes = new List<string>();

        // ---------------- USER FIELDS ----------------
        if (dto.Department != null && dto.Department != user.Department)
        {
            changes.Add($"Department: {user.Department} → {dto.Department}");
            user.Department = dto.Department;
        }

        if (dto.Designation != null && dto.Designation != user.Designation)
        {
            changes.Add($"Designation: {user.Designation} → {dto.Designation}");
            user.Designation = dto.Designation;
        }

        if (dto.EmploymentType != null && dto.EmploymentType != user.EmploymentType)
        {
            changes.Add($"EmploymentType: {user.EmploymentType} → {dto.EmploymentType}");
            user.EmploymentType = dto.EmploymentType;
        }

        if (dto.WorkShift != null && dto.WorkShift != user.WorkShift)
        {
            changes.Add($"WorkShift: {user.WorkShift} → {dto.WorkShift}");
            user.WorkShift = dto.WorkShift;
        }

        if (dto.AssignedRegion != null && dto.AssignedRegion != user.AssignedRegion)
        {
            changes.Add($"AssignedRegion: {user.AssignedRegion} → {dto.AssignedRegion}");
            user.AssignedRegion = dto.AssignedRegion;
        }

        if (dto.AssignedBranch != null && dto.AssignedBranch != user.AssignedBranch)
        {
            changes.Add($"AssignedBranch: {user.AssignedBranch} → {dto.AssignedBranch}");
            user.AssignedBranch = dto.AssignedBranch;
        }

        if (dto.Remarks != null && dto.Remarks != user.Remarks)
        {
            changes.Add("Remarks updated");
            user.Remarks = dto.Remarks;
        }

        // ---------------- PROFILE FIELDS ----------------
        if (dto.FirstName != null && dto.FirstName != user.Profile.FirstName)
        {
            changes.Add($"FirstName: {user.Profile.FirstName} → {dto.FirstName}");
            user.Profile.FirstName = dto.FirstName;
        }

        if (dto.LastName != null && dto.LastName != user.Profile.LastName)
        {
            changes.Add($"LastName: {user.Profile.LastName} → {dto.LastName}");
            user.Profile.LastName = dto.LastName;
        }

        if (dto.MobileNumber != null && dto.MobileNumber != user.Profile.MobileNumber)
        {
            changes.Add($"MobileNumber updated");
            user.Profile.MobileNumber = dto.MobileNumber;
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedBy;

        // ---------------- AUDIT LOG ----------------
        if (changes.Any())
        {
            _context.AuditLogs.Add(new AuditLog
            {
                ActorUserId = updatedBy,
                TargetUserId = userId,
                Action = "USER_UPDATE",
                Module = "USERS",
                Metadata = JsonSerializer.Serialize(new
                {
                    changes = changes
                }),

                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }



    public async Task LockUserAsync(long userId, string reason, long lockedBy)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        if (user.AccountStatus == "LOCKED")
            throw new Exception("User already locked");

        user.AccountStatus = "LOCKED";
        user.LockReason = reason;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = lockedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = lockedBy,
            TargetUserId = userId,
            Action = "USER_LOCK",
            Module = "USERS",
            Metadata = JsonSerializer.Serialize(new
            {
                reason = reason,
                lockedBy = lockedBy,
                lockedAt = DateTime.UtcNow
            }),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task UnlockUserAsync(long userId, long unlockedBy)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new Exception("User not found");

        if (user.AccountStatus != "LOCKED")
            throw new Exception("User is not locked");

        user.AccountStatus = "ACTIVE";
        user.LockReason = null;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = unlockedBy;

        _context.AuditLogs.Add(new AuditLog
        {
            ActorUserId = unlockedBy,
            TargetUserId = userId,
            Action = "USER_UNLOCK",
            Module = "USERS",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }



    public async Task<List<AdminUserListDto>> GetAdminUsersByDomainAsync(
       string domainCode,
       string? search,
       string? status,
       string? roleCode)
    {
        var query = _context.Users
            .Where(u =>
                u.Domain.DomainCode == domainCode &&
                u.DeletedAt == null
            )
            .AsQueryable();

        // 🔍 SEARCH (name, email, department)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();

            query = query.Where(u =>
                u.Username.ToLower().Contains(term) ||
                u.Email.ToLower().Contains(term) ||
                u.Profile.FirstName.ToLower().Contains(term) ||
                u.Profile.LastName.ToLower().Contains(term) ||
                (u.Department != null && u.Department.ToLower().Contains(term))
            );
        }

        // 🔒 STATUS FILTER
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(u => u.AccountStatus == status);
        }

        // 🎭 ROLE FILTER
        if (!string.IsNullOrWhiteSpace(roleCode))
        {
            query = query.Where(u =>
                u.UserRoles.Any(ur => ur.Role.RoleCode == roleCode)
            );
        }

        return await query
            .Select(u => new AdminUserListDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Email = u.Email,
                Department = u.Department,
                Designation = u.Designation,
                AccountStatus = u.AccountStatus,

                ManagerName = u.ManagerId != null
                    ? _context.Users
                        .Where(m => m.UserId == u.ManagerId)
                        .Select(m => m.Profile.FirstName + " " + m.Profile.LastName)
                        .FirstOrDefault()
                    : null,

                Roles = u.UserRoles
                    .Select(ur => ur.Role.RoleCode)
                    .ToList()
            })
            .OrderBy(u => u.Username)
            .ToListAsync();
    }
    public async Task<List<UserLookupDto>> GetAllManagersAsync()
    {
        return await _context.Users
            .Where(u =>
                u.AccountStatus == "ACTIVE" &&
                u.UserRoles.Any(ur => ur.Role.RoleCode.EndsWith("_MANAGER"))
            )
            .Select(u => new UserLookupDto
            {
                UserId = u.UserId,
                Name = u.Profile.FirstName + " " + u.Profile.LastName,
                DomainCode = u.Domain.DomainCode
            })
            .OrderBy(u => u.Name)
            .ToListAsync();
    }









}
