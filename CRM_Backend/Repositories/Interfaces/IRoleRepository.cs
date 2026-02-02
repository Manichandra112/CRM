using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role> CreateAsync(
        string roleName,
        string roleCode,
        string? description,
        long domainId,
        bool isSystemRole
    );

    Task<List<Role>> GetAllAsync();

    Task<long> GetRoleIdByCodeAsync(string roleCode);

    Task<List<Role>> GetByDomainIdAsync(long domainId);

    Task<Role?> GetByIdAsync(long id);

    Task UpdateAsync(Role role);

    Task<List<Role>> GetByCodesAsync(IEnumerable<string> roleCodes);
}
