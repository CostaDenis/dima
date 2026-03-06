using Dima.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
