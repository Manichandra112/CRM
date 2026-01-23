using CRM_Backend.DTOs.Roles;

namespace CRM_Backend.Services.Interfaces;

public interface IRoleService
{
    Task<RoleResponseDto> CreateAsync(CreateRoleDto dto);

    Task<List<RoleResponseDto>> GetAllAsync();

    // ✅ NEW (optional filter)
    Task<List<RoleResponseDto>> GetByDomainAsync(string domainCode);
}