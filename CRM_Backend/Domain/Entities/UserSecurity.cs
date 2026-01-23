using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("user_security")]
public class UserSecurity
{
    [Key, ForeignKey("User")]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("force_password_reset")]
    public bool ForcePasswordReset { get; set; }

    [Column("password_last_changed_at")]
    public DateTime? PasswordLastChangedAt { get; set; }

    [Column("password_expiry_date")]
    public DateTime? PasswordExpiryDate { get; set; }

    [Column("mfa_enabled")]
    public bool MfaEnabled { get; set; }

    [Column("mfa_type"), MaxLength(30)]
    public string? MfaType { get; set; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("last_login_ip"), MaxLength(45)]
    public string? LastLoginIp { get; set; }

    [Column("last_login_device"), MaxLength(150)]
    public string? LastLoginDevice { get; set; }

    [Column("failed_login_count")]
    public int FailedLoginCount { get; set; }

    [Column("locked_until")]
    public DateTime? LockedUntil { get; set; }

    [Column("last_login_location"), MaxLength(150)]
    public string? LastLoginLocation { get; set; }

    [Column("password_reset_token_hash"), MaxLength(255)]
    public string? PasswordResetTokenHash { get; set; }

    [Column("password_reset_expires_at")]
    public DateTime? PasswordResetExpiresAt { get; set; }

    public User User { get; set; }
}
