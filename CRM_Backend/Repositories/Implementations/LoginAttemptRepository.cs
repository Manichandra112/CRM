using CRM_Backend.Data;
using CRM_Backend.Domain.Entities;

using CRM_Backend.Repositories.Interfaces;

namespace CRM_Backend.Repositories.Implementations;

public class LoginAttemptRepository : ILoginAttemptRepository
{
    private readonly CrmAuthDbContext _context;

    public LoginAttemptRepository(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(LoginAttempt attempt)
    {
        _context.LoginAttempts.Add(attempt);
        await _context.SaveChangesAsync();
    }
}
