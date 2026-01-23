using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_Backend.Domain.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Column("employee_id"), MaxLength(50)]
    public string? EmployeeId { get; set; }

    [Required, Column("username"), MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required, Column("email"), MaxLength(150)]
    public string Email { get; set; } = null!;

    [Column("department"), MaxLength(100)]
    public string? Department { get; set; }

    [Column("designation"), MaxLength(100)]
    public string? Designation { get; set; }

    // --------------------------------------------------
    // Manager (self-reference)
    // --------------------------------------------------
    [Column("manager_id")]
    public long? ManagerId { get; set; }

    // ✅ Navigation to manager (NO DB CHANGE)
    [ForeignKey(nameof(ManagerId))]
    public User? Manager { get; set; }

    // ✅ Reverse navigation: manager → team
    public ICollection<User> TeamMembers { get; set; }
        = new List<User>();

    // --------------------------------------------------
    // Other assignments
    // --------------------------------------------------
    [Column("assigned_team_id")]
    public long? AssignedTeamId { get; set; }

    [Column("assigned_region"), MaxLength(100)]
    public string? AssignedRegion { get; set; }

    [Column("assigned_branch"), MaxLength(100)]
    public string? AssignedBranch { get; set; }

    [Required, Column("account_status"), MaxLength(30)]
    public string AccountStatus { get; set; } = "ACTIVE";

    [Column("lock_reason"), MaxLength(255)]
    public string? LockReason { get; set; }

    [Column("access_start_date")]
    public DateTime? AccessStartDate { get; set; }

    [Column("access_end_date")]
    public DateTime? AccessEndDate { get; set; }

    [Column("last_activity_at")]
    public DateTime? LastActivityAt { get; set; }

    [Column("last_assigned_lead_at")]
    public DateTime? LastAssignedLeadAt { get; set; }

    [Column("last_closed_ticket_at")]
    public DateTime? LastClosedTicketAt { get; set; }

    [Column("employment_type"), MaxLength(30)]
    public string? EmploymentType { get; set; }

    [Column("work_shift"), MaxLength(30)]
    public string? WorkShift { get; set; }

    [Column("terms_accepted_at")]
    public DateTime? TermsAcceptedAt { get; set; }

    [Column("remarks"), MaxLength(255)]
    public string? Remarks { get; set; }

    [Column("created_by")]
    public long? CreatedBy { get; set; }

    [Column("updated_by")]
    public long? UpdatedBy { get; set; }

    [Column("approved_by")]
    public long? ApprovedBy { get; set; }

    [Column("security_reviewed_by")]
    public long? SecurityReviewedBy { get; set; }

    [Required, Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("created_via"), MaxLength(30)]
    public string? CreatedVia { get; set; } = "MANAGER";

  
    public UserSecurity Security { get; set; } = null!;
    public UserProfile Profile { get; set; } = null!;

    public ICollection<UserPassword> Passwords { get; set; }
        = new List<UserPassword>();

    public ICollection<UserRole> UserRoles { get; set; }
        = new List<UserRole>();

    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();


    [Column("DomainId")]
    public long DomainId { get; set; }

    public Domain Domain { get; set; } = null!;
}
