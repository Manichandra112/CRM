using CRM_Backend.DTOs.Permissions;
using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Services.Interfaces;

public interface IPermissionService
{
    Task<long> CreateAsync(CreatePermissionDto dto);
    Task<List<Permission>> GetAllAsync();

    Task UpdateAsync(long id, UpdatePermissionDto dto);
}
