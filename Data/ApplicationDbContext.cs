using gpos.Models;
using Microsoft.EntityFrameworkCore;

namespace gpos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(user => user.Id);

                entity.Property(user => user.Id).HasColumnName("id");
                entity.Property(user => user.Username).HasColumnName("username");
                entity.Property(user => user.Email).HasColumnName("email");
                entity.Property(user => user.PasswordHash).HasColumnName("password_hash");
                entity.Property(user => user.CreatedAt).HasColumnName("created_at");
                entity.Property(user => user.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(user => user.Username).IsUnique();
                entity.HasIndex(user => user.Email).IsUnique();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(role => role.Id);

                entity.Property(role => role.Id).HasColumnName("id");
                entity.Property(role => role.Name).HasColumnName("name");
                entity.Property(role => role.Code).HasColumnName("code");
                entity.Property(role => role.CreatedAt).HasColumnName("created_at");
                entity.Property(role => role.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(role => role.Code).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions");
                entity.HasKey(permission => permission.Id);

                entity.Property(permission => permission.Id).HasColumnName("id");
                entity.Property(permission => permission.Name).HasColumnName("name");
                entity.Property(permission => permission.Code).HasColumnName("code");
                entity.Property(permission => permission.ParentId).HasColumnName("parent_id");
                entity.Property(permission => permission.CreatedAt).HasColumnName("created_at");
                entity.Property(permission => permission.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(permission => permission.Code).IsUnique();
                entity.HasOne(permission => permission.Parent)
                    .WithMany(permission => permission.Children)
                    .HasForeignKey(permission => permission.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_roles");
                entity.HasKey(userRole => userRole.Id);

                entity.Property(userRole => userRole.Id).HasColumnName("id");
                entity.Property(userRole => userRole.UserId).HasColumnName("user_id");
                entity.Property(userRole => userRole.RoleId).HasColumnName("role_id");
                entity.Property(userRole => userRole.CreatedAt).HasColumnName("created_at");
                entity.Property(userRole => userRole.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(userRole => new { userRole.UserId, userRole.RoleId }).IsUnique();
                entity.HasOne(userRole => userRole.User)
                    .WithMany(user => user.UserRoles)
                    .HasForeignKey(userRole => userRole.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(userRole => userRole.Role)
                    .WithMany(role => role.UserRoles)
                    .HasForeignKey(userRole => userRole.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions");
                entity.HasKey(rolePermission => rolePermission.Id);

                entity.Property(rolePermission => rolePermission.Id).HasColumnName("id");
                entity.Property(rolePermission => rolePermission.RoleId).HasColumnName("role_id");
                entity.Property(rolePermission => rolePermission.PermissionId).HasColumnName("permission_id");
                entity.Property(rolePermission => rolePermission.CreatedAt).HasColumnName("created_at");
                entity.Property(rolePermission => rolePermission.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId }).IsUnique();
                entity.HasOne(rolePermission => rolePermission.Role)
                    .WithMany(role => role.RolePermissions)
                    .HasForeignKey(rolePermission => rolePermission.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rolePermission => rolePermission.Permission)
                    .WithMany(permission => permission.RolePermissions)
                    .HasForeignKey(rolePermission => rolePermission.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
