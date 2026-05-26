using eCommerceSolution.OrdersService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace eCommerceSolution.OrdersService.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Maps the Order entity directly to a collection named "Orders"
        modelBuilder.Entity<Order>().ToCollection("Orders");
    }
}
