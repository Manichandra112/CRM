using CRM_Backend.DTOs.Auth;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controller.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ---------------- LOGIN ----------------

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginAsync(
            request,
            ipAddress,
            userAgent
        );

        if (!result.Success)
            return Unauthorized(new { message = result.Error });

        return Ok(result.Data);
    }

    // ---------------- CHANGE PASSWORD (AUTHENTICATED) ----------------

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = long.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        await _authService.ChangePasswordAsync(
            userId,
            dto.CurrentPassword,
            dto.NewPassword
        );

        return Ok(new
        {
            message = "Password changed successfully. Please login again."
        });
    }

    // ---------------- FORGOT PASSWORD (NEW) ----------------

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequestDto dto)
    {
        await _authService.ForgotPasswordAsync(dto.Email);

        // Always generic response (no user enumeration)
        return Ok(new
        {
            message = "If the email exists, a reset link has been sent."
        });
    }

    // ---------------- RESET FORGOT PASSWORD (NEW) ----------------

    [HttpPost("reset-forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetForgotPassword(
        ResetForgotPasswordDto dto)
    {
        await _authService.ResetForgotPasswordAsync(
            dto.Token,
            dto.NewPassword
        );

        return Ok(new
        {
            message = "Password reset successful. Please login."
        });
    }
}
