using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [Column("refresh_token_id")]
    public long RefreshTokenId { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Required, Column("token_hash"), MaxLength(255)]
    public string TokenHash { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("ip_address"), MaxLength(45)]
    public string? IpAddress { get; set; }

    [Column("user_agent"), MaxLength(255)]
    public string? UserAgent { get; set; }

    [Column("device_fingerprint"), MaxLength(255)]
    public string? DeviceFingerprint { get; set; }

    public User? User { get; set; }
}
