namespace CRM_Backend.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(
            long? actorUserId,
            long? targetUserId,
            string action,
            string module,
            object? metadata = null
        );
    }
}
