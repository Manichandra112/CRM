using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Roles;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;

namespace CRM_Backend.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roles;
    private readonly IDomainRepository _domains;

    public RoleService(IRoleRepository roles, IDomainRepository domains)
    {
        _roles = roles;
        _domains = domains;
    }

    private static RoleResponseDto Map(Role role)
    {
        return new RoleResponseDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            RoleCode = role.RoleCode,
            Description = role.Description,
            DomainId = role.DomainId,
            IsSystemRole = role.IsSystemRole,
            Active = role.Active
        };
    }

    public async Task<RoleResponseDto> CreateAsync(CreateRoleDto dto)
    {
        var domainCode = dto.RoleCode.Split('_')[0];

        var domain = await _domains.GetByCodeAsync(domainCode)
            ?? throw new Exception($"Domain '{domainCode}' not found");

        var isSystemRole = domain.DomainCode == "SYSTEM";

        var role = await _roles.CreateAsync(
            dto.RoleName,
            dto.RoleCode,
            dto.Description,
            domain.DomainId,
            isSystemRole
        );

        return Map(role);
    }

    // ✅ EXISTING: get all roles
    public async Task<List<RoleResponseDto>> GetAllAsync()
    {
        var roles = await _roles.GetAllAsync();
        return roles.Select(Map).ToList();
    }

    // ✅ NEW: get roles by domain
    public async Task<List<RoleResponseDto>> GetByDomainAsync(string domainCode)
    {
        var domain = await _domains.GetByCodeAsync(domainCode)
            ?? throw new Exception($"Domain '{domainCode}' not found");

        var roles = await _roles.GetByDomainIdAsync(domain.DomainId);
        return roles.Select(Map).ToList();
    }

    public async Task UpdateAsync(long id, UpdateRoleDto dto)
    {
        var role = await _roles.GetByIdAsync(id);

        if (role == null)
            throw new Exception("Role not found");

        if (role.IsSystemRole)
            throw new Exception("System roles cannot be modified");

        role.Active = dto.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _roles.UpdateAsync(role);
    }


}
