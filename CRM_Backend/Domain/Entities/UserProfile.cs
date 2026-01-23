using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CRM_Backend.Domain.Entities;
[Table("user_profile")]
public class UserProfile
{
    [Key, ForeignKey("User")]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("first_name"), MaxLength(100)]
    public string? FirstName { get; set; }

    [Column("last_name"), MaxLength(100)]
    public string? LastName { get; set; }

    [Column("gender"), MaxLength(20)]
    public string? Gender { get; set; }

    [Column("mobile_number"), MaxLength(20)]
    public string? MobileNumber { get; set; }

    [Column("address_line1"), MaxLength(255)]
    public string? AddressLine1 { get; set; }

    [Column("city"), MaxLength(100)]
    public string? City { get; set; }

    [Column("state"), MaxLength(100)]
    public string? State { get; set; }

    [Column("country"), MaxLength(100)]
    public string? Country { get; set; }

    [Column("postal_code"), MaxLength(20)]
    public string? PostalCode { get; set; }

    [Column("language_preference"), MaxLength(20)]
    public string? LanguagePreference { get; set; }

    [Column("timezone"), MaxLength(50)]
    public string? Timezone { get; set; }

    public User User { get; set; }
}
