using Microsoft.AspNetCore.Authorization;

public static class PermissionPolicies
{
    public static void Register(AuthorizationOptions options)
    {
        // 🔥 SUPER ADMIN ACCESS
        options.AddPolicy("CRM_FULL_ACCESS",
            PermissionPolicy.Require("CRM_FULL_ACCESS"));

        // 👤 USER
        options.AddPolicy("USER_CREATE",
            PermissionPolicy.Require("USER_CREATE"));

        options.AddPolicy("USER_UPDATE",
            PermissionPolicy.Require("USER_UPDATE"));

        options.AddPolicy("USER_VIEW",
            PermissionPolicy.Require("USER_VIEW"));

        // 👥 EMPLOYEES (GLOBAL, ZOHO-LIKE)
        options.AddPolicy("EMP_VIEW",
            PermissionPolicy.Require("EMP_VIEW"));

        // 📦 MODULE-SPECIFIC (OPTIONAL)
        options.AddPolicy("HR_EMP_VIEW",
            PermissionPolicy.Require("HR_EMP_VIEW"));

        options.AddPolicy("SALES_LEAD_VIEW",
            PermissionPolicy.Require("SALES_LEAD_VIEW"));

        options.AddPolicy("SOCIAL_POST_CREATE",
            PermissionPolicy.Require("SOCIAL_POST_CREATE"));

        options.AddPolicy("PASSWORD_RESET_COMPLETED", policy =>
        {
            policy.Requirements.Add(new ForcePasswordResetRequirement());
        });
    }
}
