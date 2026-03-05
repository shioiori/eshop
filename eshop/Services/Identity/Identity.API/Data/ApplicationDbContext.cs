using Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ApplicationRolePermission> RolePermissions => Set<ApplicationRolePermission>();
    public DbSet<ApplicationUserPermission> UserPermissions => Set<ApplicationUserPermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationRolePermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Permissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.RoleId, e.Permission }).IsUnique();
        });

        builder.Entity<ApplicationUserPermission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Permissions)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.UserId, e.Permission }).IsUnique();
        });

        // OpenIddict tables
        builder.UseOpenIddict();
    }
}
