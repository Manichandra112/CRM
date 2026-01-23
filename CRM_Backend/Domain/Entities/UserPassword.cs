using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("user_passwords")]
public class UserPassword
{
    [Key]
    [Column("password_id")]
    public long PasswordId { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Required, Column("password_hash"), MaxLength(255)]
    public string PasswordHash { get; set; }

    [Column("is_current")]
    public bool IsCurrent { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
}
