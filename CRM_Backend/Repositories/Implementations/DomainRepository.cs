using CRM_Backend.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntity = CRM_Backend.Domain.Entities.Domain;
using CRM_Backend.Repositories.Interfaces;

namespace CRM_Backend.Repositories.Implementations
{
    public class DomainRepository : IDomainRepository
    {
        private readonly CrmAuthDbContext _context;

        public DomainRepository(CrmAuthDbContext context)
        {
            _context = context;
        }

        public async Task<DomainEntity> CreateAsync(string code, string name)
        {
            if (await _context.Domains.AnyAsync(d => d.DomainCode == code))
                throw new Exception("Domain already exists");

            var domain = new DomainEntity
            {
                DomainCode = code,
                DomainName = name,
                Active = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Domains.Add(domain);
            await _context.SaveChangesAsync();
            return domain;
        }

      
        public async Task<List<DomainEntity>> GetAllAsync()
        {
            return await _context.Domains
                .OrderBy(d => d.DomainName)
                .ToListAsync();
        }

        public async Task<DomainEntity?> GetByCodeAsync(string code)
        {
            return await _context.Domains
                .FirstOrDefaultAsync(d => d.DomainCode == code && d.Active);
        }

        public async Task<DomainEntity?> GetByIdAsync(long id)
        {
            return await _context.Domains
                .FirstOrDefaultAsync(d => d.DomainId == id);
        }

        public async Task UpdateAsync(DomainEntity domain)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<DomainEntity>> GetByIdsAsync(IEnumerable<long> domainIds)
        {
            return await _context.Domains
                .Where(d => domainIds.Contains(d.DomainId) && d.Active)
                .ToListAsync();
        }


    }
}
