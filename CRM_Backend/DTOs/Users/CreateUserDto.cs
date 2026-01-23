namespace CRM_Backend.DTOs.Users;

public class CreateUserDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DomainCode { get; set; } = null!;   // HR / SALES / SOCIALMEDIA

    public string TemporaryPassword { get; set; } = null!;

    public List<string> RoleCodes { get; set; } = new();

    public UserProfileDto Profile { get; set; } = new();
}
