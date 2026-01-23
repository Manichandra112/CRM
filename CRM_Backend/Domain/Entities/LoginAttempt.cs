using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Backend.Domain.Entities;
[Table("login_attempts")]
public class LoginAttempt
{
    [Key]
    [Column("attempt_id")]
    public long AttemptId { get; set; }

    [Column("user_id")]
    public long? UserId { get; set; }

    [Column("email"), MaxLength(150)]
    public string? Email { get; set; }

    [Column("ip_address"), MaxLength(45)]
    public string? IpAddress { get; set; }

    [Column("user_agent"), MaxLength(255)]
    public string? UserAgent { get; set; }

    [Column("is_success")]
    public bool IsSuccess { get; set; }

    [Column("failure_reason"), MaxLength(255)]
    public string? FailureReason { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}
