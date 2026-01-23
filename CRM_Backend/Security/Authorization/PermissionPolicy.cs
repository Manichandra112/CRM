using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public static class PermissionPolicy
{
    public static AuthorizationPolicy Require(string permission)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAssertion(context =>
                context.User.HasClaim("perm", permission) ||
                context.User.HasClaim("perm", "CRM_FULL_ACCESS")
            )
            .Build();
    }
}
