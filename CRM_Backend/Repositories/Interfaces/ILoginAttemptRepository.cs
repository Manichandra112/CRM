using CRM_Backend.Domain.Entities;


namespace CRM_Backend.Repositories.Interfaces;

public interface ILoginAttemptRepository
{
    Task AddAsync(LoginAttempt attempt);
}
