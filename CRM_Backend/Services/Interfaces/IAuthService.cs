using CRM_Backend.DTOs.Auth;

namespace CRM_Backend.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> LoginAsync(
        LoginRequestDto request,
        string ipAddress,
        string userAgent
    );

    Task ChangePasswordAsync(
        long userId,
        string currentPassword,
        string newPassword
    );

   
    Task ForgotPasswordAsync(string email);

    Task ResetForgotPasswordAsync(
        string token,
        string newPassword
    );
}
