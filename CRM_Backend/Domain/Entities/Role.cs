using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Backend.Domain.Entities;

[Table("roles")]
public class Role
{
    [Key]
    [Column("role_id")]
    public long RoleId { get; set; }

    [Required, Column("role_name"), MaxLength(100)]
    public string RoleName { get; set; } = null!;

    [Required, Column("role_code"), MaxLength(50)]
    public string RoleCode { get; set; } = null!;

    [Column("description"), MaxLength(255)]
    public string? Description { get; set; }

    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("is_system_role")]
    public bool IsSystemRole { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

  
    public ICollection<RolePermission> RolePermissions { get; set; }
        = new List<RolePermission>();

    [Column("domain_id")]
    public long DomainId { get; set; }

    [ForeignKey(nameof(DomainId))]
    public Domain Domain { get; set; } = null!;
}
