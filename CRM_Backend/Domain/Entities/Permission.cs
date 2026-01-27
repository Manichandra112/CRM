using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("permissions")]
public class Permission
{
    [Key]
    [Column("permission_id")]
    public long PermissionId { get; set; }

    [Required, Column("permission_code"), MaxLength(100)]
    public string PermissionCode { get; set; }

    [Column("description"), MaxLength(255)]
    public string? Description { get; set; }

    [Column("module"), MaxLength(50)]
    public string? Module { get; set; }

    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }


    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
