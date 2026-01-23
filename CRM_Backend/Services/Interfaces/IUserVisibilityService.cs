namespace CRM_Backend.Services.Interfaces;

public interface IUserVisibilityService
{
    Task<List<long>> GetVisibleUserIdsAsync(long viewerUserId);
}
