using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("role_permissions")]
public class RolePermission
{
    [Column("role_id")]
    public long RoleId { get; set; }

    [Column("permission_id")]
    public long PermissionId { get; set; }

    [Column("assigned_at")]
    public DateTime AssignedAt { get; set; }

    [Column("assigned_by")]
    public long? AssignedBy { get; set; }

    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
