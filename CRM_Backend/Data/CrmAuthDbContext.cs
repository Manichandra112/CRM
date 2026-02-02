using Microsoft.EntityFrameworkCore;
using CRM_Backend.Domain.Entities;

// Alias to avoid Domain name collision
using DomainEntity = CRM_Backend.Domain.Entities.Domain;

namespace CRM_Backend.Data;

public class CrmAuthDbContext : DbContext
{
    public CrmAuthDbContext(DbContextOptions<CrmAuthDbContext> options)
        : base(options) { }

    // -----------------------------
    // Core tables
    // -----------------------------
    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserSecurity> UserSecurity => Set<UserSecurity>();
    public DbSet<UserPassword> UserPasswords => Set<UserPassword>();

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();

    // CRM Domains (HR, SALES, SOCIAL, etc.)
    public DbSet<DomainEntity> Domains => Set<DomainEntity>();

    // Departments (Engineering, HR, Sales, etc.)
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ------------------------------------------------
        // DOMAIN
        // ------------------------------------------------
        modelBuilder.Entity<DomainEntity>(entity =>
        {
            entity.ToTable("domains");

            entity.HasKey(d => d.DomainId);

            entity.Property(d => d.DomainCode)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(d => d.DomainName)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(d => d.Active)
                  .HasDefaultValue(true);

            entity.Property(d => d.CreatedAt)
                  .IsRequired();

            entity.HasIndex(d => d.DomainCode)
                  .IsUnique();
        });

        // ------------------------------------------------
        // DEPARTMENTS (domain-scoped, organizational)
        // ------------------------------------------------
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");

            entity.HasKey(d => d.Id);

            entity.Property(d => d.DomainCode)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(d => d.Code)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(d => d.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(d => d.IsActive)
                  .HasDefaultValue(true);

            entity.Property(d => d.CreatedAt)
                  .IsRequired();

            // Unique per domain
            entity.HasIndex(d => new { d.DomainCode, d.Code })
                  .IsUnique();

            // FK via business key (DomainCode)
            entity.HasOne<DomainEntity>()
                  .WithMany()
                  .HasForeignKey(d => d.DomainCode)
                  .HasPrincipalKey(d => d.DomainCode)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ------------------------------------------------
        // USER
        // ------------------------------------------------
        modelBuilder.Entity<User>()
            .HasOne(u => u.Domain)
            .WithMany()
            .HasForeignKey(u => u.DomainId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany(m => m.TeamMembers)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // ------------------------------------------------
        // ROLE
        // ------------------------------------------------
        modelBuilder.Entity<Role>()
            .HasOne(r => r.Domain)
            .WithMany(d => d.Roles)
            .HasForeignKey(r => r.DomainId)
            .OnDelete(DeleteBehavior.Restrict);

        // ------------------------------------------------
        // ROLE ↔ PERMISSION
        // ------------------------------------------------
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // ------------------------------------------------
        // USER ↔ ROLE
        // ------------------------------------------------
        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // ------------------------------------------------
        // AUDIT LOGS (append-only)
        // ------------------------------------------------
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_logs");

            entity.HasKey(a => a.AuditId);

            entity.Property(a => a.Action)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(a => a.Module)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(a => a.Metadata)
                  .HasColumnType("jsonb");

            entity.Property(a => a.CreatedAt)
                  .IsRequired();

            entity.HasIndex(a => a.ActorUserId);
            entity.HasIndex(a => a.TargetUserId);
        });
    }
}
