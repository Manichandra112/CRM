namespace CRM_Backend.Services.Implementations
{
    using CRM_Backend.Data;
    using CRM_Backend.Domain.Entities;
    using CRM_Backend.Services.Interfaces;
    using System.Text.Json;

    public class AuditLogService : IAuditLogService
    {
        private readonly CrmAuthDbContext _context;

        public AuditLogService(CrmAuthDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(
            long? actorUserId,
            long? targetUserId,
            string action,
            string module,
            object? metadata = null)
        {
            var audit = new AuditLog
            {
                ActorUserId = actorUserId,
                TargetUserId = targetUserId,
                Action = action,
                Module = module,
                Metadata = metadata == null
                    ? null
                    : JsonSerializer.Serialize(metadata),
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(audit);
            await _context.SaveChangesAsync();
        }
    }

}
