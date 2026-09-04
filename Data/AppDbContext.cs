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
    }

}