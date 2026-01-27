using DomainEntity = CRM_Backend.Domain.Entities.Domain;

namespace CRM_Backend.Repositories.Interfaces
{
    public interface IDomainRepository
    {
        Task<DomainEntity> CreateAsync(string code, string name);
        Task<List<DomainEntity>> GetAllAsync();
        Task<DomainEntity?> GetByCodeAsync(string code);

        Task<DomainEntity?> GetByIdAsync(long id);
        Task UpdateAsync(DomainEntity domain);
    }
}
