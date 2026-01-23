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
        });


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

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Domain)
            .WithMany(d => d.Roles)
            .HasForeignKey(r => r.DomainId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();
    }
}
