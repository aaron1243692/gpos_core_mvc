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
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<Tank> Tanks { get; set; }
        public DbSet<Pump> Pumps { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBatch> ProductBatches { get; set; }
        public DbSet<DisplayStock> DisplayStocks { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }

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

            modelBuilder.Entity<Fuel>(entity =>
            {
                entity.ToTable("fuels");
                entity.HasKey(fuel => fuel.Id);

                entity.Property(fuel => fuel.Id).HasColumnName("id");
                entity.Property(fuel => fuel.Name).HasColumnName("name").IsRequired();
                entity.Property(fuel => fuel.Code).HasColumnName("code");
                entity.Property(fuel => fuel.SupplierId).HasColumnName("supplier_id");
                entity.Property(fuel => fuel.CurrentPricePerLiter).HasColumnName("current_price_per_liter").HasPrecision(18, 2);
                entity.Property(fuel => fuel.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(fuel => fuel.CreatedAt).HasColumnName("created_at");
                entity.Property(fuel => fuel.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(fuel => fuel.SupplierId);
                entity.HasOne(fuel => fuel.Supplier)
                    .WithMany(supplier => supplier.Fuels)
                    .HasForeignKey(fuel => fuel.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");
                entity.HasKey(supplier => supplier.Id);

                entity.Property(supplier => supplier.Id).HasColumnName("id");
                entity.Property(supplier => supplier.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
                entity.Property(supplier => supplier.Email).HasColumnName("email").HasMaxLength(150);
                entity.Property(supplier => supplier.ContactPerson).HasColumnName("contact_person").HasMaxLength(150);
                entity.Property(supplier => supplier.ContactNumber).HasColumnName("contact_number").HasMaxLength(50);
                entity.Property(supplier => supplier.Address).HasColumnName("address").HasMaxLength(255);
                entity.Property(supplier => supplier.CreatedAt).HasColumnName("created_at");
                entity.Property(supplier => supplier.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Tank>(entity =>
            {
                entity.ToTable("tanks");
                entity.HasKey(tank => tank.Id);

                entity.Property(tank => tank.Id).HasColumnName("id");
                entity.Property(tank => tank.FuelId).HasColumnName("fuel_id");
                entity.Property(tank => tank.TankNo).HasColumnName("tank_no").IsRequired();
                entity.Property(tank => tank.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(tank => tank.CreatedAt).HasColumnName("created_at");
                entity.Property(tank => tank.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(tank => tank.FuelId);
                entity.HasOne(tank => tank.Fuel)
                    .WithMany(fuel => fuel.Tanks)
                    .HasForeignKey(tank => tank.FuelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Pump>(entity =>
            {
                entity.ToTable("pumps");
                entity.HasKey(pump => pump.Id);

                entity.Property(pump => pump.Id).HasColumnName("id");
                entity.Property(pump => pump.TankId).HasColumnName("tank_id");
                entity.Property(pump => pump.PumpNo).HasColumnName("pump_no").IsRequired();
                entity.Property(pump => pump.Name).HasColumnName("name");
                entity.Property(pump => pump.CreatedAt).HasColumnName("created_at");
                entity.Property(pump => pump.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(pump => pump.TankId);
                entity.HasOne(pump => pump.Tank)
                    .WithMany(tank => tank.Pumps)
                    .HasForeignKey(pump => pump.TankId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("product_categories");
                entity.HasKey(category => category.Id);

                entity.Property(category => category.Id).HasColumnName("id");
                entity.Property(category => category.Name).HasColumnName("name").IsRequired();
                entity.Property(category => category.Description).HasColumnName("description");
                entity.Property(category => category.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(category => category.CreatedAt).HasColumnName("created_at");
                entity.Property(category => category.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(product => product.Id);

                entity.Property(product => product.Id).HasColumnName("id");
                entity.Property(product => product.CategoryId).HasColumnName("category_id");
                entity.Property(product => product.Name).HasColumnName("name").IsRequired();
                entity.Property(product => product.IsActive).HasColumnName("is_active");
                entity.Property(product => product.CreatedAt).HasColumnName("created_at");
                entity.Property(product => product.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(product => product.CategoryId);
                entity.HasOne(product => product.Category)
                    .WithMany(category => category.Products)
                    .HasForeignKey(product => product.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductBatch>(entity =>
            {
                entity.ToTable("product_batches");
                entity.HasKey(batch => batch.Id);

                entity.Property(batch => batch.Id).HasColumnName("id");
                entity.Property(batch => batch.ProductId).HasColumnName("product_id");
                entity.Property(batch => batch.BatchNo).HasColumnName("batch_no").HasMaxLength(100).IsRequired();
                entity.Property(batch => batch.CostPrice).HasColumnName("cost_price").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(batch => batch.SellingPrice).HasColumnName("selling_price").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(batch => batch.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(batch => batch.CreatedAt).HasColumnName("created_at");
                entity.Property(batch => batch.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(batch => batch.BatchNo).IsUnique();
                entity.HasIndex(batch => batch.ProductId);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_product_batches_cost_price_non_negative", "cost_price >= 0");
                    table.HasCheckConstraint("CK_product_batches_selling_price_non_negative", "selling_price >= 0");
                });
                entity.HasOne(batch => batch.Product)
                    .WithMany(product => product.ProductBatches)
                    .HasForeignKey(batch => batch.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<WarehouseStock>(entity =>
            {
                entity.ToTable("warehouse_stocks");
                entity.HasKey(stock => stock.Id);

                entity.Property(stock => stock.Id).HasColumnName("id");
                entity.Property(stock => stock.ProductId).HasColumnName("product_id");
                entity.Property(stock => stock.BatchId).HasColumnName("batch_id");
                entity.Property(stock => stock.Quantity).HasColumnName("quantity").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(stock => stock.CreatedAt).HasColumnName("created_at");
                entity.Property(stock => stock.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(stock => stock.ProductId);
                entity.HasIndex(stock => stock.BatchId);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_warehouse_stocks_quantity_non_negative", "quantity >= 0");
                });
                entity.HasOne(stock => stock.Product)
                    .WithMany(product => product.WarehouseStocks)
                    .HasForeignKey(stock => stock.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(stock => stock.Batch)
                    .WithMany(batch => batch.WarehouseStocks)
                    .HasForeignKey(stock => stock.BatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DisplayStock>(entity =>
            {
                entity.ToTable("display_stocks");
                entity.HasKey(stock => stock.Id);

                entity.Property(stock => stock.Id).HasColumnName("id");
                entity.Property(stock => stock.ProductId).HasColumnName("product_id");
                entity.Property(stock => stock.BatchId).HasColumnName("batch_id");
                entity.Property(stock => stock.Quantity).HasColumnName("quantity").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(stock => stock.CreatedAt).HasColumnName("created_at");
                entity.Property(stock => stock.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(stock => stock.ProductId);
                entity.HasIndex(stock => stock.BatchId);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_display_stocks_quantity_non_negative", "quantity >= 0");
                });
                entity.HasOne(stock => stock.Product)
                    .WithMany(product => product.DisplayStocks)
                    .HasForeignKey(stock => stock.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(stock => stock.Batch)
                    .WithMany(batch => batch.DisplayStocks)
                    .HasForeignKey(stock => stock.BatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
