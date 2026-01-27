namespace CRM_Backend.DTOs.Users
{
    public class AdminUserDetailsDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string AccountStatus { get; set; } = null!;
        public string? LockReason { get; set; }

        public AdminUserProfileDto Profile { get; set; } = null!;
        public AdminUserOrganizationDto Organization { get; set; } = null!;

        public List<string> Roles { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
    }
}
