using FreshBake.API.Models;
using Microsoft.EntityFrameworkCore;
using FreshBake.API.Common.Enums;

namespace FreshBake.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext ( DbContextOptions<AppDbContext> options )
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CustomerProduct> CustomerProducts { get; set; }
    public DbSet<CustomerProductCategory> CustomerProductCategories { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Page> Pages { get; set; }
    public DbSet<RolePagePermission> RolePagePermissions { get; set; }

    protected override void OnModelCreating ( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<User>()
            .Property(u => u.AccessLevelId)
            .HasDefaultValue((int)AccessLevels.Customer);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Customer -> AppliedByUser (who submitted the application).
        // Not unique — application-layer logic can decide whether to
        // block a user submitting twice while a prior one is Pending.
        modelBuilder.Entity<Customer>()
            .HasOne(c => c.AppliedByUser)
            .WithMany()
            .HasForeignKey(c => c.AppliedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User -> Customer (many users can belong to one business,
        // set only on approval or when a customer-admin adds staff).
        modelBuilder.Entity<User>()
            .HasOne(u => u.Customer)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CustomerProduct>()
            .HasKey(cp => new { cp.CustomerId, cp.ProductId });

        modelBuilder.Entity<CustomerProduct>()
            .HasOne(cp => cp.Customer)
            .WithMany(c => c.CustomerProducts)
            .HasForeignKey(cp => cp.CustomerId);

        modelBuilder.Entity<CustomerProduct>()
            .HasOne(cp => cp.Product)
            .WithMany()
            .HasForeignKey(cp => cp.ProductId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductCategory)
            .WithMany(pc => pc.Products)
            .HasForeignKey(p => p.ProductCategoryId);

        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory { ProductCategoryId = 1, Name = "Cakes" },
            new ProductCategory { ProductCategoryId = 2, Name = "Pies" },
            new ProductCategory { ProductCategoryId = 3, Name = "Breads" },
            new ProductCategory { ProductCategoryId = 4, Name = "Pastries" },
            new ProductCategory { ProductCategoryId = 5, Name = "Cupcakes" },
            new ProductCategory { ProductCategoryId = 6, Name = "Cookies & Biscuits" }
        );

        modelBuilder.Entity<Product>().HasData(
            // Cakes
            new Product { ProductId = 1, Name = "Chocolate Cake", ProductCategoryId = 1 },
            new Product { ProductId = 2, Name = "Vanilla Sponge Cake", ProductCategoryId = 1 },
            new Product { ProductId = 3, Name = "Red Velvet Cake", ProductCategoryId = 1 },
            new Product { ProductId = 4, Name = "Carrot Cake", ProductCategoryId = 1 },

            // Pies
            new Product { ProductId = 5, Name = "Apple Pie", ProductCategoryId = 2 },
            new Product { ProductId = 6, Name = "Milk Tart", ProductCategoryId = 2 },
            new Product { ProductId = 7, Name = "Lemon Meringue Pie", ProductCategoryId = 2 },

            // Breads
            new Product { ProductId = 8, Name = "White Bread Loaf", ProductCategoryId = 3 },
            new Product { ProductId = 9, Name = "Whole Wheat Loaf", ProductCategoryId = 3 },
            new Product { ProductId = 10, Name = "Baguette", ProductCategoryId = 3 },
            new Product { ProductId = 11, Name = "Bread Rolls", ProductCategoryId = 3 },

            // Pastries
            new Product { ProductId = 12, Name = "Croissants", ProductCategoryId = 4 },
            new Product { ProductId = 13, Name = "Danish Pastries", ProductCategoryId = 4 },
            new Product { ProductId = 14, Name = "Vetkoek", ProductCategoryId = 4 },

            // Cupcakes
            new Product { ProductId = 15, Name = "Assorted Cupcakes", ProductCategoryId = 5 },
            new Product { ProductId = 16, Name = "Cupcake Tower (Event)", ProductCategoryId = 5 },

            // Cookies & Biscuits
            new Product { ProductId = 17, Name = "Chocolate Chip Cookies", ProductCategoryId = 6 },
            new Product { ProductId = 18, Name = "Shortbread", ProductCategoryId = 6 },
            new Product { ProductId = 19, Name = "Rusks", ProductCategoryId = 6 }
        );

        modelBuilder.Entity<CustomerProductCategory>()
            .HasKey(cpc => new { cpc.CustomerId, cpc.ProductCategoryId });

        modelBuilder.Entity<CustomerProductCategory>()
            .HasOne(cpc => cpc.Customer)
            .WithMany(c => c.CustomerProductCategories)
            .HasForeignKey(cpc => cpc.CustomerId);

        modelBuilder.Entity<CustomerProductCategory>()
            .HasOne(cpc => cpc.ProductCategory)
            .WithMany()
            .HasForeignKey(cpc => cpc.ProductCategoryId);

        modelBuilder.Entity<Role>()
    .HasOne(r => r.AccessLevel)
    .WithMany()
    .HasForeignKey(r => r.AccessLevelId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RolePagePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PageId });

        modelBuilder.Entity<RolePagePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePagePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePagePermission>()
            .HasOne(rp => rp.Page)
            .WithMany(p => p.RolePagePermissions)
            .HasForeignKey(rp => rp.PageId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        // Page seed — unchanged from before
        modelBuilder.Entity<Page>().HasData(
            new Page { PageId = 1, Name = "Dashboard", RouteKey = "Dashboard" },
            new Page { PageId = 2, Name = "Manage Users", RouteKey = "ManageUsers" },
            new Page { PageId = 3, Name = "Manage Customers", RouteKey = "ManageCustomers" },
            new Page { PageId = 4, Name = "Customer Manual Registration", RouteKey = "CustomerManualRegistration" },
            new Page { PageId = 5, Name = "Manage Products", RouteKey = "ManageProducts" },
            new Page { PageId = 6, Name = "Manage Product Categories", RouteKey = "ManageProductCategories" },
            new Page { PageId = 7, Name = "User Permissions", RouteKey = "UserPermissions" },
            new Page { PageId = 8, Name = "My Details", RouteKey = "MyDetails" },
            new Page { PageId = 9, Name = "Customer Self Registration", RouteKey = "CustomerSelfRegistration" }
        );

        // Role seed — each Role now points at an existing AccessLevel row.
        // Assumes AccessLevel.AccessLevelId values match your enum (1=Admin, 2=Customer, 3=SuperUser, 4=User).
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, Name = "Admin", AccessLevelId = 1 },
            new Role { RoleId = 2, Name = "Super User", AccessLevelId = 3 },
            new Role { RoleId = 3, Name = "Customer", Description = "Base customer access", AccessLevelId = 2 },
            new Role { RoleId = 4, Name = "Customer Manager", Description = "Business-level management access", AccessLevelId = 2 },
            new Role { RoleId = 5, Name = "Customer Employee", Description = "Limited business staff access", AccessLevelId = 2 },
            new Role { RoleId = 6, Name = "User", Description = "Generic, not-yet-elevated account", AccessLevelId = 4 }
        );

        // Permission seed — same shape as before, just one extra role (RoleId 6)
        modelBuilder.Entity<RolePagePermission>().HasData(
            // Admin — everything
            new RolePagePermission { RoleId = 1, PageId = 1, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 2, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 3, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 4, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 5, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 6, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 7, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 8, CanView = true },
            new RolePagePermission { RoleId = 1, PageId = 9, CanView = true },

            // Super User — everything
            new RolePagePermission { RoleId = 2, PageId = 1, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 2, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 3, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 4, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 5, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 6, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 7, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 8, CanView = true },
            new RolePagePermission { RoleId = 2, PageId = 9, CanView = true },

            // Customer (base) — Dashboard, My Details, Self Registration
            new RolePagePermission { RoleId = 3, PageId = 1, CanView = true },
            new RolePagePermission { RoleId = 3, PageId = 8, CanView = true },
            new RolePagePermission { RoleId = 3, PageId = 9, CanView = true },

            // Customer Manager — Dashboard, My Details (Admin allocates more later)
            new RolePagePermission { RoleId = 4, PageId = 1, CanView = true },
            new RolePagePermission { RoleId = 4, PageId = 8, CanView = true },

            // Customer Employee — Dashboard, My Details
            new RolePagePermission { RoleId = 5, PageId = 1, CanView = true },
            new RolePagePermission { RoleId = 5, PageId = 8, CanView = true },

            // User (generic) — Dashboard, My Details
            new RolePagePermission { RoleId = 6, PageId = 1, CanView = true },
            new RolePagePermission { RoleId = 6, PageId = 8, CanView = true }


        );

        modelBuilder.Entity<AccessLevel>().HasData(
            new AccessLevel { AccessLevelId = 1, AccessLevelName = "Admin" },
            new AccessLevel { AccessLevelId = 2, AccessLevelName = "Customer" },
            new AccessLevel { AccessLevelId = 3, AccessLevelName = "SuperUser" },
            new AccessLevel { AccessLevelId = 4, AccessLevelName = "User" }
        );
    }

}