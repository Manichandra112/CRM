//using CRM_Backend.DTOs.Auth;
//using CRM_Backend.Services.Interfaces;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace CRM_Backend.Controller.Auth;

//[ApiController]
//[Route("api/auth")]
//public class AuthController : ControllerBase
//{
//    private readonly IAuthService _authService;

//    public AuthController(IAuthService authService)
//    {
//        _authService = authService;
//    }

//    // ---------------- LOGIN ----------------

//    [HttpPost("login")]
//    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
//    {
//        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
//        var userAgent = Request.Headers.UserAgent.ToString();

//        var result = await _authService.LoginAsync(
//            request,
//            ipAddress,
//            userAgent
//        );

//        if (!result.Success)
//            return Unauthorized(new { message = result.Error });

//        return Ok(result.Data);
//    }

//    // ---------------- CHANGE PASSWORD (AUTHENTICATED) ----------------

//    [HttpPost("change-password")]
//    [Authorize]
//    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
//    {
//        var userId = long.Parse(
//            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
//        );

//        await _authService.ChangePasswordAsync(
//            userId,
//            dto.NewPassword
//        );

//        return Ok(new
//        {
//            message = "Password set successfully. Please login again."
//        });
//    }


//    // ---------------- FORGOT PASSWORD  ----------------

//    [HttpPost("forgot-password")]
//    [AllowAnonymous]
//    public async Task<IActionResult> ForgotPassword(
//        ForgotPasswordRequestDto dto)
//    {
//        await _authService.ForgotPasswordAsync(dto.Email);

//        // Always generic response (no user enumeration)
//        return Ok(new
//        {
//            message = "If the email exists, a reset link has been sent."
//        });
//    }

//    // ---------------- Validting token  ----------------
//    [HttpGet("validate-reset-token")]
//    [AllowAnonymous]
//    public async Task<IActionResult> ValidateResetToken(
//    [FromQuery] string token)
//    {
//        if (string.IsNullOrWhiteSpace(token))
//        {
//            return BadRequest(new { valid = false });
//        }

//        var isValid = await _authService.ValidateResetTokenAsync(token);

//        if (!isValid)
//        {
//            return BadRequest(new { valid = false });
//        }

//        return Ok(new { valid = true });
//    }

//    // ---------------- RESET FORGOT PASSWORD (NEW) ----------------

//    [HttpPost("reset-forgot-password")]
//    [AllowAnonymous]
//    public async Task<IActionResult> ResetForgotPassword(
//        ResetForgotPasswordDto dto)
//    {
//        await _authService.ResetForgotPasswordAsync(
//            dto.Token,
//            dto.NewPassword
//        );

//        return Ok(new
//        {
//            message = "Password reset successful. Please login."
//        });
//    }


//}


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
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";
        var userAgent = Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginAsync(
            request,
            ipAddress,
            userAgent
        );

        // Success only — failures are handled by middleware
        return Ok(result);
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
            dto.NewPassword
        );

        return Ok(new
        {
            message = "Password set successfully. Please login again."
        });
    }

    // ---------------- FORGOT PASSWORD ----------------

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto dto)
    {
        await _authService.ForgotPasswordAsync(dto.Email);

        // Always generic response (no user enumeration)
        return Ok(new
        {
            message = "If the email exists, a reset link has been sent."
        });
    }

    // ---------------- VALIDATE RESET TOKEN ----------------

    [HttpGet("validate-reset-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { valid = false });

        var isValid = await _authService.ValidateResetTokenAsync(token);

        return isValid
            ? Ok(new { valid = true })
            : BadRequest(new { valid = false });
    }

    // ---------------- RESET FORGOT PASSWORD ----------------

    [HttpPost("reset-forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetForgotPassword(ResetForgotPasswordDto dto)
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
