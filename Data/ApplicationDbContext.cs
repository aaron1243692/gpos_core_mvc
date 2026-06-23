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
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBatch> ProductBatches { get; set; }
        public DbSet<StockReceiving> StockReceivings { get; set; }
        public DbSet<StockReceivingItem> StockReceivingItems { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<LowStockSetting> LowStockSettings { get; set; }
        public DbSet<DisplayStock> DisplayStocks { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public DbSet<Nozzle> Nozzles { get; set; }
        public DbSet<FuelDelivery> FuelDeliveries { get; set; }
        public DbSet<FuelPriceHistory> FuelPriceHistory { get; set; }
        public DbSet<PumpMeterReading> PumpMeterReadings { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<RebateRule> RebateRules { get; set; }
        public DbSet<PointsLedger> PointsLedger { get; set; }
        public DbSet<DiscountRule> DiscountRules { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<EmployeeAccount> EmployeeAccounts { get; set; }
        public DbSet<StationSetting> StationSettings { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleDetail> ScheduleDetails { get; set; }
        public DbSet<ShiftSetting> ShiftSettings { get; set; }
        public DbSet<EmployeeShiftSchedule> EmployeeShiftSchedules { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

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
                entity.Property(user => user.FullName).HasColumnName("full_name").HasMaxLength(150);
                entity.Property(user => user.ContactNumber).HasColumnName("contact_number").HasMaxLength(50);
                entity.Property(user => user.Address).HasColumnName("address").HasMaxLength(255);
                entity.Property(user => user.BranchId).HasColumnName("branch_id");
                entity.Property(user => user.DepartmentId).HasColumnName("department_id");
                entity.Property(user => user.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(user => user.CreatedAt).HasColumnName("created_at");
                entity.Property(user => user.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(user => user.Username).IsUnique();
                entity.HasIndex(user => user.Email).IsUnique();
                entity.HasIndex(user => user.BranchId);
                entity.HasIndex(user => user.DepartmentId);
                entity.HasOne(user => user.Branch)
                    .WithMany()
                    .HasForeignKey(user => user.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(user => user.Department)
                    .WithMany()
                    .HasForeignKey(user => user.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");
                entity.HasKey(role => role.Id);

                entity.Property(role => role.Id).HasColumnName("id");
                entity.Property(role => role.Name).HasColumnName("name");
                entity.Property(role => role.Code).HasColumnName("code");
                entity.Property(role => role.Status).HasColumnName("status").HasDefaultValue(1);
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
                entity.Property(permission => permission.Status).HasColumnName("status").HasDefaultValue(1);
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
                entity.Property(rolePermission => rolePermission.Status).HasColumnName("status").HasDefaultValue(1);
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
                entity.Property(fuel => fuel.Status).HasColumnName("status").HasDefaultValue(1);
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
                entity.Property(supplier => supplier.Status).HasColumnName("status").HasDefaultValue(1);
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
                entity.Property(tank => tank.CapacityLiters).HasColumnName("capacity_liters").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(tank => tank.CurrentLiters).HasColumnName("current_liters").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(tank => tank.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(tank => tank.Status).HasColumnName("status").HasDefaultValue(1);
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
                entity.Property(pump => pump.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(pump => pump.CreatedAt).HasColumnName("created_at");
                entity.Property(pump => pump.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(pump => pump.TankId);
                entity.HasOne(pump => pump.Tank)
                    .WithMany(tank => tank.Pumps)
                    .HasForeignKey(pump => pump.TankId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Nozzle>(entity =>
            {
                entity.ToTable("nozzles");
                entity.HasKey(nozzle => nozzle.Id);

                entity.Property(nozzle => nozzle.Id).HasColumnName("id");
                entity.Property(nozzle => nozzle.PumpId).HasColumnName("pump_id");
                entity.Property(nozzle => nozzle.NozzleNo).HasColumnName("nozzle_no").HasMaxLength(100).IsRequired();
                entity.Property(nozzle => nozzle.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(nozzle => nozzle.CreatedAt).HasColumnName("created_at");
                entity.Property(nozzle => nozzle.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(nozzle => nozzle.PumpId);
                entity.HasOne(nozzle => nozzle.Pump)
                    .WithMany(pump => pump.Nozzles)
                    .HasForeignKey(nozzle => nozzle.PumpId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FuelDelivery>(entity =>
            {
                entity.ToTable("fuel_deliveries");
                entity.HasKey(delivery => delivery.Id);

                entity.Property(delivery => delivery.Id).HasColumnName("id");
                entity.Property(delivery => delivery.DeliveryNo).HasColumnName("delivery_no").HasMaxLength(100).IsRequired();
                entity.Property(delivery => delivery.SupplierId).HasColumnName("supplier_id");
                entity.Property(delivery => delivery.FuelId).HasColumnName("fuel_id");
                entity.Property(delivery => delivery.TankId).HasColumnName("tank_id");
                entity.Property(delivery => delivery.DeliveredLiters).HasColumnName("delivered_liters").HasPrecision(18, 2);
                entity.Property(delivery => delivery.CostPerLiter).HasColumnName("cost_per_liter").HasPrecision(18, 2);
                entity.Property(delivery => delivery.TotalCost).HasColumnName("total_cost").HasPrecision(18, 2);
                entity.Property(delivery => delivery.DeliveryDate).HasColumnName("delivery_date");
                entity.Property(delivery => delivery.Remarks).HasColumnName("remarks").HasMaxLength(255);
                entity.Property(delivery => delivery.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(delivery => delivery.CreatedAt).HasColumnName("created_at");
                entity.Property(delivery => delivery.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(delivery => delivery.DeliveryNo).IsUnique();
                entity.HasIndex(delivery => delivery.SupplierId);
                entity.HasIndex(delivery => delivery.FuelId);
                entity.HasIndex(delivery => delivery.TankId);
                entity.HasOne(delivery => delivery.Supplier)
                    .WithMany(supplier => supplier.FuelDeliveries)
                    .HasForeignKey(delivery => delivery.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(delivery => delivery.Fuel)
                    .WithMany(fuel => fuel.FuelDeliveries)
                    .HasForeignKey(delivery => delivery.FuelId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(delivery => delivery.Tank)
                    .WithMany(tank => tank.FuelDeliveries)
                    .HasForeignKey(delivery => delivery.TankId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FuelPriceHistory>(entity =>
            {
                entity.ToTable("fuel_price_history");
                entity.HasKey(history => history.Id);

                entity.Property(history => history.Id).HasColumnName("id");
                entity.Property(history => history.FuelId).HasColumnName("fuel_id");
                entity.Property(history => history.OldPrice).HasColumnName("old_price").HasPrecision(18, 2);
                entity.Property(history => history.NewPrice).HasColumnName("new_price").HasPrecision(18, 2);
                entity.Property(history => history.EffectiveAt).HasColumnName("effective_at");
                entity.Property(history => history.Remarks).HasColumnName("remarks").HasMaxLength(255);
                entity.Property(history => history.CreatedBy).HasColumnName("created_by");
                entity.Property(history => history.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(history => history.CreatedAt).HasColumnName("created_at");
                entity.Property(history => history.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(history => history.FuelId);
                entity.HasOne(history => history.Fuel)
                    .WithMany(fuel => fuel.FuelPriceHistory)
                    .HasForeignKey(history => history.FuelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PumpMeterReading>(entity =>
            {
                entity.ToTable("pump_meter_readings");
                entity.HasKey(reading => reading.Id);

                entity.Property(reading => reading.Id).HasColumnName("id");
                entity.Property(reading => reading.PumpId).HasColumnName("pump_id");
                entity.Property(reading => reading.NozzleId).HasColumnName("nozzle_id");
                entity.Property(reading => reading.ShiftId).HasColumnName("shift_id");
                entity.Property(reading => reading.OpeningMeter).HasColumnName("opening_meter").HasPrecision(18, 2);
                entity.Property(reading => reading.ClosingMeter).HasColumnName("closing_meter").HasPrecision(18, 2);
                entity.Property(reading => reading.LitersSold).HasColumnName("liters_sold").HasPrecision(18, 2);
                entity.Property(reading => reading.ReadingDate).HasColumnName("reading_date");
                entity.Property(reading => reading.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(reading => reading.CreatedAt).HasColumnName("created_at");
                entity.Property(reading => reading.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(reading => reading.PumpId);
                entity.HasIndex(reading => reading.NozzleId);
                entity.HasOne(reading => reading.Pump)
                    .WithMany(pump => pump.PumpMeterReadings)
                    .HasForeignKey(reading => reading.PumpId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(reading => reading.Nozzle)
                    .WithMany(nozzle => nozzle.PumpMeterReadings)
                    .HasForeignKey(reading => reading.NozzleId)
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
                entity.Property(category => category.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(category => category.CreatedAt).HasColumnName("created_at");
                entity.Property(category => category.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(product => product.Id);

                entity.Property(product => product.Id).HasColumnName("id");
                entity.Property(product => product.CategoryId).HasColumnName("category_id");
                entity.Property(product => product.ProductUnitId).HasColumnName("product_unit_id");
                entity.Property(product => product.Name).HasColumnName("name").IsRequired();
                entity.Property(product => product.IsActive).HasColumnName("is_active");
                entity.Property(product => product.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(product => product.CreatedAt).HasColumnName("created_at");
                entity.Property(product => product.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(product => product.CategoryId);
                entity.HasIndex(product => product.ProductUnitId);
                entity.HasOne(product => product.Category)
                    .WithMany(category => category.Products)
                    .HasForeignKey(product => product.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(product => product.ProductUnit)
                    .WithMany(unit => unit.Products)
                    .HasForeignKey(product => product.ProductUnitId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductUnit>(entity =>
            {
                entity.ToTable("product_units");
                entity.HasKey(unit => unit.Id);

                entity.Property(unit => unit.Id).HasColumnName("id");
                entity.Property(unit => unit.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(unit => unit.Abbreviation).HasColumnName("abbreviation").HasMaxLength(30);
                entity.Property(unit => unit.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(unit => unit.CreatedAt).HasColumnName("created_at");
                entity.Property(unit => unit.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<ProductBatch>(entity =>
            {
                entity.ToTable("product_batches");
                entity.HasKey(batch => batch.Id);

                entity.Property(batch => batch.Id).HasColumnName("id");
                entity.Property(batch => batch.ProductId).HasColumnName("product_id");
                entity.Property(batch => batch.SupplierId).HasColumnName("supplier_id");
                entity.Property(batch => batch.BatchNo).HasColumnName("batch_no").HasMaxLength(100).IsRequired();
                entity.Property(batch => batch.CostPrice).HasColumnName("cost_price").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(batch => batch.SellingPrice).HasColumnName("selling_price").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(batch => batch.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(batch => batch.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(batch => batch.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(batch => batch.CreatedAt).HasColumnName("created_at");
                entity.Property(batch => batch.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(batch => batch.BatchNo).IsUnique();
                entity.HasIndex(batch => batch.ProductId);
                entity.HasIndex(batch => batch.SupplierId);
                entity.ToTable(table =>
                {
                    table.HasCheckConstraint("CK_product_batches_cost_price_non_negative", "cost_price >= 0");
                    table.HasCheckConstraint("CK_product_batches_selling_price_non_negative", "selling_price >= 0");
                });
                entity.HasOne(batch => batch.Product)
                    .WithMany(product => product.ProductBatches)
                    .HasForeignKey(batch => batch.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(batch => batch.Supplier)
                    .WithMany(supplier => supplier.ProductBatches)
                    .HasForeignKey(batch => batch.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockReceiving>(entity =>
            {
                entity.ToTable("stock_receivings");
                entity.HasKey(receiving => receiving.Id);

                entity.Property(receiving => receiving.Id).HasColumnName("id");
                entity.Property(receiving => receiving.ReceivingNo).HasColumnName("receiving_no").HasMaxLength(100).IsRequired();
                entity.Property(receiving => receiving.SupplierId).HasColumnName("supplier_id");
                entity.Property(receiving => receiving.ReceivedDate).HasColumnName("received_date");
                entity.Property(receiving => receiving.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(receiving => receiving.Remarks).HasColumnName("remarks").HasMaxLength(255);
                entity.Property(receiving => receiving.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(receiving => receiving.CreatedAt).HasColumnName("created_at");
                entity.Property(receiving => receiving.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(receiving => receiving.ReceivingNo).IsUnique();
                entity.HasIndex(receiving => receiving.SupplierId);
                entity.HasOne(receiving => receiving.Supplier)
                    .WithMany(supplier => supplier.StockReceivings)
                    .HasForeignKey(receiving => receiving.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockReceivingItem>(entity =>
            {
                entity.ToTable("stock_receiving_items");
                entity.HasKey(item => item.Id);

                entity.Property(item => item.Id).HasColumnName("id");
                entity.Property(item => item.StockReceivingId).HasColumnName("stock_receiving_id");
                entity.Property(item => item.ProductId).HasColumnName("product_id");
                entity.Property(item => item.ProductBatchId).HasColumnName("product_batch_id");
                entity.Property(item => item.Quantity).HasColumnName("quantity").HasPrecision(18, 2);
                entity.Property(item => item.CostPrice).HasColumnName("cost_price").HasPrecision(18, 2);
                entity.Property(item => item.SellingPrice).HasColumnName("selling_price").HasPrecision(18, 2);
                entity.Property(item => item.ExpiryDate).HasColumnName("expiry_date");
                entity.Property(item => item.Subtotal).HasColumnName("subtotal").HasPrecision(18, 2);
                entity.Property(item => item.CreatedAt).HasColumnName("created_at");
                entity.Property(item => item.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(item => item.StockReceivingId);
                entity.HasIndex(item => item.ProductId);
                entity.HasIndex(item => item.ProductBatchId);
                entity.HasOne(item => item.StockReceiving)
                    .WithMany(receiving => receiving.Items)
                    .HasForeignKey(item => item.StockReceivingId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(item => item.Product)
                    .WithMany(product => product.StockReceivingItems)
                    .HasForeignKey(item => item.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(item => item.ProductBatch)
                    .WithMany(batch => batch.StockReceivingItems)
                    .HasForeignKey(item => item.ProductBatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.ToTable("stock_movements");
                entity.HasKey(movement => movement.Id);

                entity.Property(movement => movement.Id).HasColumnName("id");
                entity.Property(movement => movement.ProductId).HasColumnName("product_id");
                entity.Property(movement => movement.ProductBatchId).HasColumnName("product_batch_id");
                entity.Property(movement => movement.SourceLocation).HasColumnName("source_location").HasMaxLength(50);
                entity.Property(movement => movement.DestinationLocation).HasColumnName("destination_location").HasMaxLength(50);
                entity.Property(movement => movement.MovementType).HasColumnName("movement_type").HasMaxLength(50).IsRequired();
                entity.Property(movement => movement.Quantity).HasColumnName("quantity").HasPrecision(18, 2);
                entity.Property(movement => movement.ReferenceType).HasColumnName("reference_type").HasMaxLength(100);
                entity.Property(movement => movement.ReferenceId).HasColumnName("reference_id");
                entity.Property(movement => movement.Remarks).HasColumnName("remarks").HasMaxLength(255);
                entity.Property(movement => movement.CreatedBy).HasColumnName("created_by");
                entity.Property(movement => movement.CreatedAt).HasColumnName("created_at");

                entity.HasIndex(movement => movement.ProductId);
                entity.HasIndex(movement => movement.ProductBatchId);
                entity.HasOne(movement => movement.Product)
                    .WithMany(product => product.StockMovements)
                    .HasForeignKey(movement => movement.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(movement => movement.ProductBatch)
                    .WithMany(batch => batch.StockMovements)
                    .HasForeignKey(movement => movement.ProductBatchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LowStockSetting>(entity =>
            {
                entity.ToTable("low_stock_settings");
                entity.HasKey(setting => setting.Id);

                entity.Property(setting => setting.Id).HasColumnName("id");
                entity.Property(setting => setting.ProductId).HasColumnName("product_id");
                entity.Property(setting => setting.ProductBatchId).HasColumnName("product_batch_id");
                entity.Property(setting => setting.Location).HasColumnName("location").HasMaxLength(50).IsRequired();
                entity.Property(setting => setting.MinimumQuantity).HasColumnName("minimum_quantity").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(setting => setting.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(setting => setting.CreatedAt).HasColumnName("created_at");
                entity.Property(setting => setting.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(setting => setting.ProductId);
                entity.HasIndex(setting => setting.ProductBatchId);
                entity.HasOne(setting => setting.Product)
                    .WithMany(product => product.LowStockSettings)
                    .HasForeignKey(setting => setting.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(setting => setting.ProductBatch)
                    .WithMany(batch => batch.LowStockSettings)
                    .HasForeignKey(setting => setting.ProductBatchId)
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

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("discounts");
                entity.HasKey(discount => discount.Id);

                entity.Property(discount => discount.Id).HasColumnName("id");
                entity.Property(discount => discount.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(discount => discount.EarnRate).HasColumnName("earn_rate").HasPrecision(5, 2).HasDefaultValue(0m);
                entity.Property(discount => discount.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(discount => discount.CreatedAt).HasColumnName("created_at");
                entity.Property(discount => discount.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.ToTable("members");
                entity.HasKey(member => member.Id);

                entity.Property(member => member.Id).HasColumnName("id");
                entity.Property(member => member.MemberNo).HasColumnName("member_no").HasMaxLength(100).IsRequired();
                entity.Property(member => member.CardNo).HasColumnName("card_no").HasMaxLength(100);
                entity.Property(member => member.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
                entity.Property(member => member.ContactNumber).HasColumnName("contact_number").HasMaxLength(50);
                entity.Property(member => member.Email).HasColumnName("email").HasMaxLength(150);
                entity.Property(member => member.Address).HasColumnName("address").HasMaxLength(255);
                entity.Property(member => member.DiscountId).HasColumnName("discount_id");
                entity.Property(member => member.Points).HasColumnName("points").HasPrecision(18, 2).HasDefaultValue(0m);
                entity.Property(member => member.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(member => member.CreatedAt).HasColumnName("created_at");
                entity.Property(member => member.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(member => member.MemberNo).IsUnique();
                entity.HasIndex(member => member.CardNo).IsUnique();
                entity.HasIndex(member => member.DiscountId);
                entity.HasOne(member => member.Discount)
                    .WithMany(discount => discount.Members)
                    .HasForeignKey(member => member.DiscountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RebateRule>(entity =>
            {
                entity.ToTable("rebate_rules");
                entity.HasKey(rule => rule.Id);

                entity.Property(rule => rule.Id).HasColumnName("id");
                entity.Property(rule => rule.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(rule => rule.AppliesTo).HasColumnName("applies_to").HasMaxLength(50).IsRequired();
                entity.Property(rule => rule.PointsRequired).HasColumnName("points_required").HasPrecision(18, 2);
                entity.Property(rule => rule.RebateValue).HasColumnName("rebate_value").HasPrecision(18, 2);
                entity.Property(rule => rule.MinimumPurchase).HasColumnName("minimum_purchase").HasPrecision(18, 2);
                entity.Property(rule => rule.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(rule => rule.CreatedAt).HasColumnName("created_at");
                entity.Property(rule => rule.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<PointsLedger>(entity =>
            {
                entity.ToTable("points_ledger");
                entity.HasKey(ledger => ledger.Id);

                entity.Property(ledger => ledger.Id).HasColumnName("id");
                entity.Property(ledger => ledger.MemberId).HasColumnName("member_id");
                entity.Property(ledger => ledger.TransactionType).HasColumnName("transaction_type").HasMaxLength(50).IsRequired();
                entity.Property(ledger => ledger.Points).HasColumnName("points").HasPrecision(18, 2);
                entity.Property(ledger => ledger.OldPoints).HasColumnName("old_points").HasPrecision(18, 2);
                entity.Property(ledger => ledger.NewPoints).HasColumnName("new_points").HasPrecision(18, 2);
                entity.Property(ledger => ledger.ReferenceType).HasColumnName("reference_type").HasMaxLength(100);
                entity.Property(ledger => ledger.ReferenceId).HasColumnName("reference_id");
                entity.Property(ledger => ledger.Remarks).HasColumnName("remarks").HasMaxLength(255);
                entity.Property(ledger => ledger.CreatedAt).HasColumnName("created_at");

                entity.HasIndex(ledger => ledger.MemberId);
                entity.HasOne(ledger => ledger.Member)
                    .WithMany(member => member.PointsLedger)
                    .HasForeignKey(ledger => ledger.MemberId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DiscountRule>(entity =>
            {
                entity.ToTable("discount_rules");
                entity.HasKey(rule => rule.Id);

                entity.Property(rule => rule.Id).HasColumnName("id");
                entity.Property(rule => rule.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(rule => rule.DiscountId).HasColumnName("discount_id");
                entity.Property(rule => rule.DiscountType).HasColumnName("discount_type").HasMaxLength(50).IsRequired();
                entity.Property(rule => rule.DiscountValue).HasColumnName("discount_value").HasPrecision(18, 2);
                entity.Property(rule => rule.AppliesTo).HasColumnName("applies_to").HasMaxLength(50).IsRequired();
                entity.Property(rule => rule.MinimumAmount).HasColumnName("minimum_amount").HasPrecision(18, 2);
                entity.Property(rule => rule.MemberRequired).HasColumnName("member_required").HasDefaultValue(0);
                entity.Property(rule => rule.StartDate).HasColumnName("start_date");
                entity.Property(rule => rule.EndDate).HasColumnName("end_date");
                entity.Property(rule => rule.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(rule => rule.CreatedAt).HasColumnName("created_at");
                entity.Property(rule => rule.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(rule => rule.DiscountId);
                entity.HasOne(rule => rule.Discount)
                    .WithMany(discount => discount.DiscountRules)
                    .HasForeignKey(rule => rule.DiscountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("branches");
                entity.HasKey(branch => branch.Id);

                entity.Property(branch => branch.Id).HasColumnName("id");
                entity.Property(branch => branch.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(branch => branch.Address).HasColumnName("address").HasMaxLength(255);
                entity.Property(branch => branch.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(branch => branch.CreatedAt).HasColumnName("created_at");
                entity.Property(branch => branch.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");
                entity.HasKey(department => department.Id);

                entity.Property(department => department.Id).HasColumnName("id");
                entity.Property(department => department.BranchId).HasColumnName("branch_id");
                entity.Property(department => department.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(department => department.Description).HasColumnName("description").HasMaxLength(255);
                entity.Property(department => department.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(department => department.CreatedAt).HasColumnName("created_at");
                entity.Property(department => department.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(department => department.BranchId);
                entity.HasOne(department => department.Branch)
                    .WithMany(branch => branch.Departments)
                    .HasForeignKey(department => department.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("positions");
                entity.HasKey(position => position.Id);

                entity.Property(position => position.Id).HasColumnName("id");
                entity.Property(position => position.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(position => position.Description).HasColumnName("description").HasMaxLength(255);
                entity.Property(position => position.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(position => position.CreatedAt).HasColumnName("created_at");
                entity.Property(position => position.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<EmployeeAccount>(entity =>
            {
                entity.ToTable("employee_account");
                entity.HasKey(account => account.Id);

                entity.Property(account => account.Id).HasColumnName("id");
                entity.Property(account => account.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
                entity.Property(account => account.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                entity.Property(account => account.FullName).HasColumnName("full_name").HasMaxLength(150).IsRequired();
                entity.Property(account => account.Email).HasColumnName("email").HasMaxLength(150);
                entity.Property(account => account.ContactNumber).HasColumnName("contact_number").HasMaxLength(50);
                entity.Property(account => account.Address).HasColumnName("address").HasMaxLength(255);
                entity.Property(account => account.DepartmentId).HasColumnName("department_id");
                entity.Property(account => account.PositionId).HasColumnName("position_id");
                entity.Property(account => account.ScheduleId).HasColumnName("schedule_id");
                entity.Property(account => account.Role).HasColumnName("role").HasMaxLength(50);
                entity.Property(account => account.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(account => account.CreatedAt).HasColumnName("created_at");
                entity.Property(account => account.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(account => account.Username).IsUnique();
                entity.HasIndex(account => account.DepartmentId);
                entity.HasIndex(account => account.PositionId);
                entity.HasIndex(account => account.ScheduleId);
                entity.HasOne(account => account.Department)
                    .WithMany(department => department.EmployeeAccounts)
                    .HasForeignKey(account => account.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(account => account.Position)
                    .WithMany(position => position.EmployeeAccounts)
                    .HasForeignKey(account => account.PositionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(account => account.Schedule)
                    .WithMany(schedule => schedule.EmployeeAccounts)
                    .HasForeignKey(account => account.ScheduleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedules");
                entity.HasKey(schedule => schedule.Id);

                entity.Property(schedule => schedule.Id).HasColumnName("id");
                entity.Property(schedule => schedule.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(schedule => schedule.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(schedule => schedule.CreatedAt).HasColumnName("created_at");
                entity.Property(schedule => schedule.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(schedule => schedule.Name).IsUnique();
            });

            modelBuilder.Entity<ScheduleDetail>(entity =>
            {
                entity.ToTable("schedule_details");
                entity.HasKey(detail => detail.Id);

                entity.Property(detail => detail.Id).HasColumnName("id");
                entity.Property(detail => detail.ScheduleId).HasColumnName("schedule_id");
                entity.Property(detail => detail.DayOfWeek).HasColumnName("day_of_week");
                entity.Property(detail => detail.AmIn).HasColumnName("am_in").HasColumnType("time");
                entity.Property(detail => detail.AmOut).HasColumnName("am_out").HasColumnType("time");
                entity.Property(detail => detail.PmIn).HasColumnName("pm_in").HasColumnType("time");
                entity.Property(detail => detail.PmOut).HasColumnName("pm_out").HasColumnType("time");
                entity.Property(detail => detail.CreatedAt).HasColumnName("created_at");
                entity.Property(detail => detail.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(detail => detail.ScheduleId);
                entity.HasIndex(detail => new { detail.ScheduleId, detail.DayOfWeek }).IsUnique();
                entity.HasOne(detail => detail.Schedule)
                    .WithMany(schedule => schedule.Details)
                    .HasForeignKey(detail => detail.ScheduleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EmployeeShiftSchedule>(entity =>
            {
                entity.ToTable("employee_shift_schedules");
                entity.HasKey(schedule => schedule.Id);

                entity.Property(schedule => schedule.Id).HasColumnName("id");
                entity.Property(schedule => schedule.EmployeeAccountId).HasColumnName("employee_account_id");
                entity.Property(schedule => schedule.ShiftSettingId).HasColumnName("shift_setting_id");
                entity.Property(schedule => schedule.DayOfWeek).HasColumnName("day_of_week");
                entity.Property(schedule => schedule.StartTime).HasColumnName("start_time").HasColumnType("time");
                entity.Property(schedule => schedule.EndTime).HasColumnName("end_time").HasColumnType("time");
                entity.Property(schedule => schedule.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(schedule => schedule.CreatedAt).HasColumnName("created_at");
                entity.Property(schedule => schedule.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(schedule => schedule.EmployeeAccountId);
                entity.HasIndex(schedule => schedule.ShiftSettingId);
                entity.HasOne(schedule => schedule.EmployeeAccount)
                    .WithMany(account => account.ShiftSchedules)
                    .HasForeignKey(schedule => schedule.EmployeeAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(schedule => schedule.ShiftSetting)
                    .WithMany(setting => setting.EmployeeShiftSchedules)
                    .HasForeignKey(schedule => schedule.ShiftSettingId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StationSetting>(entity =>
            {
                entity.ToTable("station_settings");
                entity.HasKey(setting => setting.Id);

                entity.Property(setting => setting.Id).HasColumnName("id");
                entity.Property(setting => setting.StationName).HasColumnName("station_name").HasMaxLength(150).IsRequired();
                entity.Property(setting => setting.BusinessName).HasColumnName("business_name").HasMaxLength(150);
                entity.Property(setting => setting.Address).HasColumnName("address").HasMaxLength(255);
                entity.Property(setting => setting.Tin).HasColumnName("tin").HasMaxLength(100);
                entity.Property(setting => setting.ReceiptHeader).HasColumnName("receipt_header").HasMaxLength(255);
                entity.Property(setting => setting.ReceiptFooter).HasColumnName("receipt_footer").HasMaxLength(255);
                entity.Property(setting => setting.DefaultBranchId).HasColumnName("default_branch_id");
                entity.Property(setting => setting.Currency).HasColumnName("currency").HasMaxLength(20).HasDefaultValue("PHP");
                entity.Property(setting => setting.TaxEnabled).HasColumnName("tax_enabled").HasDefaultValue(0);
                entity.Property(setting => setting.TaxRate).HasColumnName("tax_rate").HasPrecision(5, 2).HasDefaultValue(0m);
                entity.Property(setting => setting.CreatedAt).HasColumnName("created_at");
                entity.Property(setting => setting.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(setting => setting.DefaultBranchId);
                entity.HasOne(setting => setting.DefaultBranch)
                    .WithMany()
                    .HasForeignKey(setting => setting.DefaultBranchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.ToTable("payment_methods");
                entity.HasKey(method => method.Id);

                entity.Property(method => method.Id).HasColumnName("id");
                entity.Property(method => method.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(method => method.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
                entity.Property(method => method.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(method => method.CreatedAt).HasColumnName("created_at");
                entity.Property(method => method.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(method => method.Code).IsUnique();
            });

            modelBuilder.Entity<ShiftSetting>(entity =>
            {
                entity.ToTable("shift_settings");
                entity.HasKey(setting => setting.Id);

                entity.Property(setting => setting.Id).HasColumnName("id");
                entity.Property(setting => setting.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(setting => setting.StartTime).HasColumnName("start_time").HasColumnType("time");
                entity.Property(setting => setting.EndTime).HasColumnName("end_time").HasColumnType("time");
                entity.Property(setting => setting.RequireOpeningCash).HasColumnName("require_opening_cash").HasDefaultValue(1);
                entity.Property(setting => setting.AllowCashIn).HasColumnName("allow_cash_in").HasDefaultValue(1);
                entity.Property(setting => setting.AllowCashOut).HasColumnName("allow_cash_out").HasDefaultValue(1);
                entity.Property(setting => setting.RequireClosingApproval).HasColumnName("require_closing_approval").HasDefaultValue(0);
                entity.Property(setting => setting.Status).HasColumnName("status").HasDefaultValue(1);
                entity.Property(setting => setting.CreatedAt).HasColumnName("created_at");
                entity.Property(setting => setting.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.ToTable("activity_logs");
                entity.HasKey(log => log.Id);

                entity.Property(log => log.Id).HasColumnName("id");
                entity.Property(log => log.UserId).HasColumnName("user_id");
                entity.Property(log => log.Username).HasColumnName("username").HasMaxLength(100);
                entity.Property(log => log.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
                entity.Property(log => log.Module).HasColumnName("module").HasMaxLength(100);
                entity.Property(log => log.Description).HasColumnName("description").HasMaxLength(255);
                entity.Property(log => log.IpAddress).HasColumnName("ip_address").HasMaxLength(100);
                entity.Property(log => log.CreatedAt).HasColumnName("created_at");

                entity.HasIndex(log => log.UserId);
                entity.HasOne(log => log.User)
                    .WithMany()
                    .HasForeignKey(log => log.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
