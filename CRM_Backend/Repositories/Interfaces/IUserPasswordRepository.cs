using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IUserPasswordRepository
{
    Task<UserPassword?> GetCurrentPasswordAsync(long userId);
    Task UpdateAsync(UserPassword password);
    Task AddAsync(UserPassword password);
}

