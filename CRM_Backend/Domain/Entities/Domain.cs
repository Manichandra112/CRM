using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Backend.Domain.Entities;
[Table("domains")]
public class Domain
{
    [Key]
    [Column("domain_id")]
    public long DomainId { get; set; }

    [Required]
    [Column("domain_code")]
    [MaxLength(50)]
    public string DomainCode { get; set; } = null!;   // HR, SALES, SOCIAL

    [Required]
    [Column("domain_name")]
    [MaxLength(150)]
    public string DomainName { get; set; } = null!;   // Human Resources, Sales CRM

    [Column("active")]
    public bool Active { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
