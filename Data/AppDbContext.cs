using FreshBake.API.Models;
using Microsoft.EntityFrameworkCore;

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
            .HasIndex(u => u.Email)
            .IsUnique(); // prevents duplicate accounts with the same email
    }

}