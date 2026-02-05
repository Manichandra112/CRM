using CRM_Backend.DTOs.Auth;

namespace CRM_Backend.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        string ipAddress,
        string userAgent
    );

    Task ChangePasswordAsync(
        long userId,
        string newPassword
    );

    Task<bool> ValidateResetTokenAsync(string token);

    Task ForgotPasswordAsync(string email);

    Task ResetForgotPasswordAsync(
        string token,
        string newPassword
    );
}
