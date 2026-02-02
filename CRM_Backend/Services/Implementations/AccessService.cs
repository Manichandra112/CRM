using CRM_Backend.Domain.Constants;
using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Access;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

using DomainEntity = CRM_Backend.Domain.Entities.Domain;

namespace CRM_Backend.Services.Implementations
{
    public class AccessService : IAccessService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserRoleRepository _userRoleRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IPermissionRepository _permissionRepo;
        private readonly IDomainRepository _domainRepo;

        public AccessService(
            IUserRepository userRepo,
            IUserRoleRepository userRoleRepo,
            IRoleRepository roleRepo,
            IPermissionRepository permissionRepo,
            IDomainRepository domainRepo)
        {
            _userRepo = userRepo;
            _userRoleRepo = userRoleRepo;
            _roleRepo = roleRepo;
            _permissionRepo = permissionRepo;
            _domainRepo = domainRepo;
        }

        public async Task<AccessMeResponse> GetMeAsync(long userId)
        {
            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new UnauthorizedAccessException("User not found");

            if (user.AccountStatus != AccountStatus.ACTIVE)
                throw new UnauthorizedAccessException("Account inactive");

            var roleCodes =
                await _userRoleRepo.GetRoleCodesByUserIdAsync(userId)
                ?? Enumerable.Empty<string>();

            var roles =
                roleCodes.Any()
                    ? await _roleRepo.GetByCodesAsync(roleCodes)
                    : new List<Role>();

            var permissionCodes =
                await _userRoleRepo.GetPermissionCodesByUserIdAsync(userId)
                ?? Enumerable.Empty<string>();

            var permissions =
                permissionCodes.Any()
                    ? await _permissionRepo.GetByCodesAsync(permissionCodes)
                    : new List<Permission>();

            var permissionsByModule = permissions
                .GroupBy(p => p.Module)
                .ToList();

            var domains = new Dictionary<string, DomainEntity>();

            foreach (var group in permissionsByModule)
            {
                var domain = await _domainRepo.GetByCodeAsync(group.Key);
                if (domain != null)
                {
                    domains[group.Key] = domain;
                }
            }

            // 6️⃣ BUILD DOMAIN DTOs
            var domainDtos = permissionsByModule
                .Where(g => domains.ContainsKey(g.Key))
                .Select(g =>
                {
                    var domain = domains[g.Key];
                    return new AccessDomainDto
                    {
                        Code = domain.DomainCode,
                        Name = domain.DomainName,
                        Permissions = g.Select(p => new AccessPermissionDto
                        {
                            Code = p.PermissionCode,
                            Name = p.Description
                        }).ToList()
                    };
                })
                .ToList();

            // 7️⃣ FINAL RESPONSE
            return new AccessMeResponse
            {
                User = new AccessUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    AccountStatus = user.AccountStatus,

                    // 🔐 SAFE: security row may not exist
                    PwdResetRequired = user.Security?.ForcePasswordReset ?? false
                },

                Roles = roles.Select(r => new AccessRoleDto
                {
                    Code = r.RoleCode,
                    Name = r.RoleName
                }).ToList(),

                Domains = domainDtos,

                PermissionsFlat = permissions
                    .Select(p => p.PermissionCode)
                    .Distinct()
                    .ToList()
            };
        }
    }
}
