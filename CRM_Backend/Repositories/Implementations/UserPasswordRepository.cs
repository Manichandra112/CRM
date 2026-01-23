using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;
using CRM_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Repositories.Implementations;

public class UserPasswordRepository : IUserPasswordRepository
{
    private readonly CrmAuthDbContext _context;

    public UserPasswordRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<UserPassword?> GetCurrentPasswordAsync(long userId)
    {
        return await _context.UserPasswords
            .Where(p => p.UserId == userId && p.IsCurrent)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(UserPassword password)
    {
        _context.UserPasswords.Add(password);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserPassword password)
    {
        _context.UserPasswords.Update(password);
        await _context.SaveChangesAsync();
    }
}

