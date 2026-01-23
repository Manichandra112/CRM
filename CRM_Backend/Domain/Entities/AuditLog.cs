namespace CRM_Backend.Domain.Entities;

public class AuditLog
{
    public long AuditId { get; set; }

    public long? ActorUserId { get; set; }
    public long? TargetUserId { get; set; }

    public string Action { get; set; } = null!;
    public string Module { get; set; } = null!;

    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; }
}
