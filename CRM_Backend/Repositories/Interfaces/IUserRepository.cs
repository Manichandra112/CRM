using CRM_Backend.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(long userId);
    Task AddAsync(User user);

    Task<List<User>> GetUsersByDomainCodeAsync(string domainCode);
    Task<List<User>> GetUsersByDomainIdAsync(long domainId);

    Task<List<User>> GetAllManagersAsync();
}
