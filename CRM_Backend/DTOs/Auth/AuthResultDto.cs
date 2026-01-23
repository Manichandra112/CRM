namespace CRM_Backend.DTOs.Auth;

public class AuthResultDto
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public LoginResponseDto? Data { get; set; }
}
