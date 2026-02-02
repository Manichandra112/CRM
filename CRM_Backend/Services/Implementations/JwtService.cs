//using CRM_Backend.Domain.Entities;
//using CRM_Backend.Security.Jwt;
//using CRM_Backend.Services.Interfaces;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace CRM_Backend.Services.Implementations;

//public class JwtService : IJwtService
//{
//    private readonly JwtSettings _settings;

//    public JwtService(IOptions<JwtSettings> options)
//    {
//        _settings = options.Value;
//    }

//    public string GenerateAccessToken(
//        User user,
//        bool passwordResetCompleted,
//        IEnumerable<string> roles,
//        IEnumerable<string> permissions)
//    {
//        var now = DateTime.UtcNow;

//        var claims = new List<Claim>
//        {
//            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
//            new(JwtRegisteredClaimNames.Email, user.Email),
//            new("username", user.Username),

//            // 🔐 PASSWORD RESET GATE
//            new(
//                "pwd_reset_completed",
//                passwordResetCompleted.ToString().ToLower()
//            ),

//            // ✅ ACCOUNT STATUS (Zoho-style hard gate)
//            new("account_status", user.AccountStatus),

//            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//            new(
//                JwtRegisteredClaimNames.Iat,
//                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
//                ClaimValueTypes.Integer64
//            )
//        };

//        foreach (var role in roles)
//            claims.Add(new Claim(ClaimTypes.Role, role));

//        foreach (var permission in permissions)
//            claims.Add(new Claim("perm", permission));

//        var key = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(_settings.Key)
//        );

//        var creds = new SigningCredentials(
//            key,
//            SecurityAlgorithms.HmacSha256
//        );

//        var token = new JwtSecurityToken(
//            issuer: _settings.Issuer,
//            audience: _settings.Audience,
//            claims: claims,
//            notBefore: now,
//            expires: now.AddMinutes(_settings.AccessTokenMinutes),
//            signingCredentials: creds
//        );

//        return new JwtSecurityTokenHandler().WriteToken(token);
//    }
//}




using CRM_Backend.Domain.Entities;
using CRM_Backend.Security.Jwt;
using CRM_Backend.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRM_Backend.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateAccessToken(
        User user,
        IEnumerable<string> roles,
        IEnumerable<string> permissions)
    {
        var now = DateTime.UtcNow;

        // 🔐 SOURCE OF TRUTH (matches your entity)
        var forcePasswordReset =
            user.Security != null &&
            user.Security.ForcePasswordReset;

        var claims = new List<Claim>
        {
            // 🔑 IDENTITY
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username),

            // 🔐 PASSWORD RESET FLOW
            new Claim(
                "pwd_reset_required",
                forcePasswordReset.ToString().ToLower()
            ),
            new Claim(
                "pwd_reset_completed",
                (!forcePasswordReset).ToString().ToLower()
            ),

            // 🔒 ACCOUNT STATUS HARD GATE
            new Claim("account_status", user.AccountStatus),

            // 🔁 TOKEN META
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            )
        };

        // 👤 ROLES
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 🛂 PERMISSIONS
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("perm", permission));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Key)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_settings.AccessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

