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
    protected override void OnModelCreating ( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<User>()
        .Property(u => u.AccessLevelId)
        .HasDefaultValue((int)AccessLevels.Customer);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); // Prevents duplicate accounts with the same email
    }

}