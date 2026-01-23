namespace CRM_Backend.DTOs.Auth
{
    public class ResetForgotPasswordDto
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
