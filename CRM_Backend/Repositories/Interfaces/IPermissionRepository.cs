using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IPermissionRepository
{
    Task<long> CreateAsync(string code, string description, string module);
    Task<List<Permission>> GetAllAsync();


    Task<long?> GetPermissionIdByCodeAsync(string permissionCode);
    Task<List<string>> GetPermissionCodesByIdsAsync(IEnumerable<long> permissionIds);

    Task<Permission?> GetByIdAsync(long id);
    Task UpdateAsync(Permission permission);


    Task<List<Permission>> GetByCodesAsync(IEnumerable<string> permissionCodes);


}
