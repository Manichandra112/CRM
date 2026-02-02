using CRM_Backend.DTOs.Access;

namespace CRM_Backend.Services.Interfaces
{
    public interface IAccessService
    {
        Task<AccessMeResponse> GetMeAsync(long userId);
    }
}
