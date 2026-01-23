using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("user_roles")]
public class UserRole
{
    [Key]
    [Column("user_role_id")]
    public long UserRoleId { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("role_id")]
    public long RoleId { get; set; }

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; }

    [Column("assigned_by")]
    public long? AssignedBy { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
