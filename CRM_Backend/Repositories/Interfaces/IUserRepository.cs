using CRM_Backend.Domain.Entities;

namespace CRM_Backend.Repositories.Interfaces;

public interface IUserRepository
{
    // Auth-related
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(long userId);
    Task AddAsync(User user);

    // Authorization / business reads
    Task<List<User>> GetUsersByDomainCodeAsync(string domainCode);
    Task<List<User>> GetUsersByDomainIdAsync(long domainId);
}
