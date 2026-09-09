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
    protected override void OnModelCreating ( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<User>()
            .Property(u => u.AccessLevelId)
            .HasDefaultValue((int)AccessLevels.Customer);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.UserId)
            .IsUnique(); // enforces one Customer record per User

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
    }

}