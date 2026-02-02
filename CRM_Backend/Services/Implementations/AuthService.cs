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
    private readonly IConfiguration _configuration;
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
        INotificationService notificationService,
        IConfiguration configuration)
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
        _configuration = configuration;
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

        if (currentPassword == null)
        {
            if (security?.ForcePasswordReset != true)
            {
                await _userSecurityRepository.IncrementFailedAsync(user.UserId);
                return Fail("Invalid email or password");
            }
        }
        else
        {
            if (!_passwordService.VerifyPassword(request.Password, currentPassword.PasswordHash))
            {
                await _userSecurityRepository.IncrementFailedAsync(user.UserId);
                return Fail("Invalid email or password");
            }
        }

        // 🔐 SECURITY HOUSEKEEPING
        await _userSecurityRepository.ResetFailuresAsync(user.UserId);
        await _refreshTokenRepository.RevokeAllAsync(user.UserId);

        await _userSecurityRepository.UpdateLastLoginAsync(
            user.UserId,
            ipAddress,
            userAgent
        );

        await LogAttempt(
            user.UserId,
            user.Email,
            ipAddress,
            userAgent,
            true,
            null
        );

        var roles = await _userRoleRepository.GetRoleCodesByUserIdAsync(user.UserId);
        var permissions = await _userRoleRepository.GetPermissionCodesByUserIdAsync(user.UserId);

        // ✅ JWT derives reset flags from DB (NO boolean passed anymore)
        var accessToken = _jwtService.GenerateAccessToken(
            user,
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



    public async Task ChangePasswordAsync(
    long userId,
    string newPassword)
    {
        var security = await _userSecurityRepository.GetByUserIdAsync(userId)
            ?? throw new Exception("Security record not found");

        // 🔁 Expire old password (if exists)
        var current = await _passwordRepository.GetCurrentPasswordAsync(userId);
        if (current != null)
        {
            current.IsCurrent = false;
            await _passwordRepository.UpdateAsync(current);
        }

        // ✅ Set new password
        await _passwordRepository.AddAsync(new UserPassword
        {
            UserId = userId,
            PasswordHash = _passwordService.HashPassword(newPassword),
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow
        });

        // 🔐 Clear force-reset flag (safe even if already false)
        await _userSecurityRepository.ClearForceResetAsync(userId);

        // 🔒 Kill all existing sessions
        await _refreshTokenRepository.RevokeAllAsync(userId);
    }



    // --------------------------------------------------
    // FORGOT PASSWORD
    // --------------------------------------------------
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return; // prevent user enumeration

        var rawToken = Guid.NewGuid().ToString("N");
        var tokenHash = HashResetToken(rawToken);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        await _userSecurityRepository.SetPasswordResetAsync(
            user.UserId,
            tokenHash,
            expiresAt
        );

        // ✅ READ FROM CONFIG
        var baseUrl = _configuration["Frontend:BaseUrl"]?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new Exception("Frontend BaseUrl is not configured");


        var resetLink = $"{baseUrl}/reset-forgot-password?token={rawToken}";
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

        if (security.PasswordResetExpiresAt == null ||
            security.PasswordResetExpiresAt < DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired reset token");
        }

        var userId = security.UserId;

        var current = await _passwordRepository.GetCurrentPasswordAsync(userId);

        if (current != null)
        {
            current.IsCurrent = false;
            await _passwordRepository.UpdateAsync(current);
        }

        await _passwordRepository.AddAsync(new UserPassword
        {
            UserId = userId,
            PasswordHash = _passwordService.HashPassword(newPassword),
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow
        });

        // 🔐 Clear reset state completely
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

    public async Task<bool> ValidateResetTokenAsync(string token)
    {
        var tokenHash = HashResetToken(token);

        var security = await _userSecurityRepository
            .GetByResetTokenHashAsync(tokenHash);

        if (security == null ||
            security.PasswordResetExpiresAt == null ||
            security.PasswordResetExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }


    private static AuthResultDto Fail(string message) =>
        new() { Success = false, Error = message };
}
