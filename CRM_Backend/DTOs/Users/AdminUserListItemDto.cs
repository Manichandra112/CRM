namespace CRM_Backend.DTOs.Users;

public class AdminUserListItemDto
{
    public long UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string AccountStatus { get; set; } = null!;
    public string? ManagerName { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
}
