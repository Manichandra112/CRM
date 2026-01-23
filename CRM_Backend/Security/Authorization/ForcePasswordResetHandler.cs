namespace CRM_Backend.Security.Authorization
{
    using Microsoft.AspNetCore.Authorization;
    using CRM_Backend.Repositories.Interfaces;

    public class ForcePasswordResetHandler
        : AuthorizationHandler<ForcePasswordResetRequirement>
    {
        private readonly IUserSecurityRepository _security;

        public ForcePasswordResetHandler(IUserSecurityRepository security)
        {
            _security = security;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ForcePasswordResetRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst("sub");
            if (userIdClaim == null)
                return;

            var userId = long.Parse(userIdClaim.Value);

            var security = await _security.GetByUserIdAsync(userId);
            if (security == null)
                return;

            if (!security.ForcePasswordReset)
                context.Succeed(requirement);
        }
    }

}
