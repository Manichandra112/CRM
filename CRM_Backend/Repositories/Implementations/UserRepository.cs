using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly CrmAuthDbContext _context;

    public UserRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    // ---------------- AUTH ----------------

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(long userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    // ---------------- BUSINESS / AUTHZ ----------------

    public async Task<List<User>> GetUsersByDomainCodeAsync(string domainCode)
    {
        return await _context.Users
            .Include(u => u.Domain)
            .Where(u =>
                u.Domain.DomainCode == domainCode &&
                u.AccountStatus == "ACTIVE")
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<List<User>> GetUsersByDomainIdAsync(long domainId)
    {
        return await _context.Users
            .Where(u =>
                u.DomainId == domainId &&
                u.AccountStatus == "ACTIVE")
            .OrderBy(u => u.Username)
            .ToListAsync();
    }
}
