using CRM_Backend.Domain.Entities;
using CRM_Backend.DTOs.Auth;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Security.Tokens;
using CRM_Backend.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using CRM_Backend.Domain.Constants;

namespace CRM_Backend.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserPasswordRepository _passwordRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILoginAttemptRepository _loginAttemptRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;
    private readonly IJwtService _jwtService;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly INotificationService _notificationService;

    public AuthService(
        IUserRepository userRepository,
        IUserPasswordRepository passwordRepository,
        IPasswordService passwordService,
        ILoginAttemptRepository loginAttemptRepository,
        IUserSecurityRepository userSecurityRepository,
        IJwtService jwtService,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        INotificationService notificationService)
    {
        _userRepository = userRepository;
        _passwordRepository = passwordRepository;
        _passwordService = passwordService;
        _loginAttemptRepository = loginAttemptRepository;
        _userSecurityRepository = userSecurityRepository;
        _jwtService = jwtService;
        _userRoleRepository = userRoleRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _notificationService = notificationService;
    }

    // --------------------------------------------------
    // LOGIN
    // --------------------------------------------------
    public async Task<AuthResultDto> LoginAsync(
        LoginRequestDto request,
        string ipAddress,
        string userAgent)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            await LogAttempt(null, request.Email, ipAddress, userAgent, false, "User not found");
            return Fail("Invalid email or password");
        }

        var security = await _userSecurityRepository.GetByUserIdAsync(user.UserId);

        if (security?.LockedUntil != null && security.LockedUntil > DateTime.UtcNow)
            return Fail("Account is temporarily locked");

        if (user.AccountStatus != AccountStatus.ACTIVE)
        {
            var reason = user.AccountStatus == AccountStatus.INACTIVE
                ? "Account is inactive"
                : "Account has been exited";

            await LogAttempt(
                user.UserId,
                user.Email,
                ipAddress,
                userAgent,
                false,
                reason
            );

            return Fail(reason);
        }


        var currentPassword = await _passwordRepository.GetCurrentPasswordAsync(user.UserId);

        if (currentPassword == null ||
            !_passwordService.VerifyPassword(request.Password, currentPassword.PasswordHash))
        {
            await _userSecurityRepository.IncrementFailedAsync(user.UserId);
            return Fail("Invalid email or password");
        }

        await _userSecurityRepository.ResetFailuresAsync(user.UserId);
        await _refreshTokenRepository.RevokeAllAsync(user.UserId);

        var roles = await _userRoleRepository.GetRoleCodesByUserIdAsync(user.UserId);
        var permissions = await _userRoleRepository.GetPermissionCodesByUserIdAsync(user.UserId);

        bool passwordResetCompleted = !security.ForcePasswordReset;

        var accessToken = _jwtService.GenerateAccessToken(
            user,
            passwordResetCompleted,
            roles,
            permissions
        );

        var rawRefreshToken = RefreshTokenGenerator.GenerateToken();
        var hashedRefreshToken = RefreshTokenGenerator.HashToken(rawRefreshToken);

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeviceFingerprint = $"{ipAddress}:{userAgent}".GetHashCode().ToString()
        });

        return new AuthResultDto
        {
            Success = true,
            Data = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            }
        };
    }

    // --------------------------------------------------
    // CHANGE PASSWORD (LOGGED-IN USER)
    // --------------------------------------------------
    public async Task ChangePasswordAsync(long userId, string currentPassword, string newPassword)
    {
        var current = await _passwordRepository.GetCurrentPasswordAsync(userId)
            ?? throw new Exception("Password not found");

        if (!_passwordService.VerifyPassword(currentPassword, current.PasswordHash))
            throw new Exception("Current password is incorrect");

        current.IsCurrent = false;
        await _passwordRepository.UpdateAsync(current);

        await _passwordRepository.AddAsync(new UserPassword
        {
            UserId = userId,
            PasswordHash = _passwordService.HashPassword(newPassword),
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow
        });

        await _userSecurityRepository.ClearForceResetAsync(userId);
    }

    // --------------------------------------------------
    // FORGOT PASSWORD
    // --------------------------------------------------
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return; // prevent enumeration

        var rawToken = Guid.NewGuid().ToString("N");
        var tokenHash = HashResetToken(rawToken);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        await _userSecurityRepository.SetPasswordResetAsync(
            user.UserId,
            tokenHash,
            expiresAt
        );

        var resetLink = $"https://frontend/reset-password?token={rawToken}";

        await _notificationService.SendPasswordResetAsync(
            user.Email,
            resetLink
        );
    }

    // --------------------------------------------------
    // RESET FORGOT PASSWORD
    // --------------------------------------------------
    public async Task ResetForgotPasswordAsync(string token, string newPassword)
    {
        var tokenHash = HashResetToken(token);

        var security = await _userSecurityRepository
            .GetByResetTokenHashAsync(tokenHash);

        if (security == null)
            throw new Exception("Invalid or expired reset token");

        var userId = security.UserId;

        var current = await _passwordRepository.GetCurrentPasswordAsync(userId);

        // If a password exists, expire it
        if (current != null)
        {
            current.IsCurrent = false;
            await _passwordRepository.UpdateAsync(current);
        }

        // Always create a new password
        await _passwordRepository.AddAsync(new UserPassword
        {
            UserId = userId,
            PasswordHash = _passwordService.HashPassword(newPassword),
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow
        });

        await _userSecurityRepository.ClearPasswordResetAsync(userId);
        await _refreshTokenRepository.RevokeAllAsync(userId);
    }


    // --------------------------------------------------
    // HELPERS
    // --------------------------------------------------
    private static string HashResetToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    private async Task LogAttempt(
        long? userId,
        string email,
        string ipAddress,
        string userAgent,
        bool isSuccess,
        string? failureReason)
    {
        await _loginAttemptRepository.AddAsync(new LoginAttempt
        {
            UserId = userId,
            Email = email,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccess = isSuccess,
            FailureReason = failureReason,
            CreatedAt = DateTime.UtcNow
        });
    }

    private static AuthResultDto Fail(string message) =>
        new() { Success = false, Error = message };
}
