using CRM_Backend.Data;
using CRM_Backend.Repositories.Implementations;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Email;
using CRM_Backend.Security.Jwt;
using CRM_Backend.Services.Implementations;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Controllers
// --------------------------------------------------
builder.Services.AddControllers();

// --------------------------------------------------
// Swagger
// --------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM_Backend",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --------------------------------------------------
// Database (PostgreSQL)
// --------------------------------------------------
builder.Services.AddDbContext<CrmAuthDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// --------------------------------------------------
// API behavior
// --------------------------------------------------
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// --------------------------------------------------
// CORS (lock down in prod)
// --------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --------------------------------------------------
// JWT Settings
// --------------------------------------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email")
);

// --------------------------------------------------
// Authentication (JWT)
// --------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // dev only
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),

        ClockSkew = TimeSpan.Zero
    };
});

// --------------------------------------------------
// Authorization (permission-based)
// --------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    
    PermissionPolicies.Register(options);

   
    options.AddPolicy("PASSWORD_RESET_COMPLETED", policy =>
        policy.RequireClaim("pwd_reset_completed", "true")
    );

    options.AddPolicy("ACCOUNT_ACTIVE", policy =>
        policy.RequireClaim("account_status", "ACTIVE")
    );

   
    options.AddPolicy("USER_VIEW", policy =>
        policy.RequireClaim("perm", "USER_VIEW")
    );

    options.AddPolicy("USER_CREATE", policy =>
        policy.RequireClaim("perm", "USER_CREATE")
    );

    options.AddPolicy("USER_UPDATE", policy =>
        policy.RequireClaim("perm", "USER_UPDATE")
    );

    options.AddPolicy("USER_LOCK", policy =>
        policy.RequireClaim("perm", "USER_LOCK")
    );

    options.AddPolicy("USER_UNLOCK", policy =>
        policy.RequireClaim("perm", "USER_UNLOCK")
    );

    options.AddPolicy("USER_ASSIGN_MANAGER", policy =>
        policy.RequireClaim("perm", "USER_ASSIGN_MANAGER")
    );

    options.AddPolicy("USER_RESET_PASSWORD", policy =>
        policy.RequireClaim("perm", "USER_RESET_PASSWORD")
    );

   
    options.AddPolicy("CRM_FULL_ACCESS", policy =>
        policy.RequireClaim("perm", "CRM_FULL_ACCESS")
    );
});






// --------------------------------------------------
// Dependency Injection
// --------------------------------------------------

builder.Services.AddScoped<IAuthorizationHandler,
    ForcePasswordResetHandler>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));

// Services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDomainService, DomainService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserVisibilityService, UserVisibilityService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAdminUserListService, AdminUserListService>();
builder.Services.AddScoped<IAdminUserDetailsService, AdminUserDetailsService>();
builder.Services.AddScoped<IAdminUserSecurityService, AdminUserSecurityService>();
builder.Services.AddScoped<IAdminUserAuditLogService, AdminUserAuditLogService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAccessService, AccessService>();





// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserPasswordRepository, UserPasswordRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserSecurityRepository, UserSecurityRepository>();
builder.Services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDomainRepository, DomainRepository>();

// --------------------------------------------------
var app = builder.Build();

// --------------------------------------------------
// Middleware pipeline
// --------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCors");

app.UseRouting();
app.UseHttpsRedirection();

// 🔐 ORDER MATTERS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
