using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Security.Tokens;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Auth;

[ApiController]
[Route("api/token")]
public class TokenController : ControllerBase
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly IUserSecurityRepository _userSecurityRepo;

    public TokenController(
        IRefreshTokenRepository refreshRepo,
        IJwtService jwtService,
        IUserRepository userRepo,
        IUserRoleRepository userRoleRepo,
        IUserSecurityRepository userSecurityRepo)
    {
        _refreshRepo = refreshRepo;
        _jwtService = jwtService;
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _userSecurityRepo = userSecurityRepo;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var tokenHash = RefreshTokenGenerator.HashToken(refreshToken);
        var stored = await _refreshRepo.GetByHashAsync(tokenHash);

        if (stored == null)
            return Unauthorized("Invalid refresh token");

        var fingerprint =
            $"{Request.HttpContext.Connection.RemoteIpAddress}:{Request.Headers.UserAgent}"
            .GetHashCode()
            .ToString();

        if (stored.DeviceFingerprint != fingerprint)
            return Unauthorized("Device mismatch");

        // rotate refresh token
        await _refreshRepo.RevokeAsync(stored.RefreshTokenId);

        var user = await _userRepo.GetByIdAsync(stored.UserId);
        var roles = await _userRoleRepo.GetRoleCodesByUserIdAsync(user.UserId);
        var perms = await _userRoleRepo.GetPermissionCodesByUserIdAsync(user.UserId);

        var security = await _userSecurityRepo.GetByUserIdAsync(user.UserId);
        bool passwordResetCompleted = !security.ForcePasswordReset;

        var newJwt = _jwtService.GenerateAccessToken(
            user,
            passwordResetCompleted,
            roles,
            perms
        );

        return Ok(new { accessToken = newJwt });
    }
}
