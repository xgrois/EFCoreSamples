// See https://aka.ms/new-console-template for more information

using CA.M2M.Modern.Entities;
using Microsoft.EntityFrameworkCore;

namespace CA.M2M.Modern.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Dish> Dishes => Set<Dish>();

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Instead of defining here all property tables and fields we just read all
        // from each IEntityTypeConfiguration<T> derived class
        // This way is crearer and prevents this method to grow
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantDbContext).Assembly);
    }
}
