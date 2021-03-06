# ManyToMany in modern approach with EFCore5+

In the modern approach, you don't need to implement the join table in your data model.

Advantages:

-   Data model is simpler. Only 2 models for a M2M relationship.
-   Data model is "more OOP like". Intermediate objects don't make sense in OOP.
-   Queries are simpler (no need to be aware of join table)

Disadvantages:

-   I think you cannot define the name of the join table. It might be something you want for naming conventions.
For instance, in this sample, the generated table name breaks naming conventions.
-   You may prefer that your data model fits with the DB model and you don't like to assume "hidden" tables.

Limitations:

-   If you need to include extra properties in your DB join table beyond the 2 FKs,
    you cannot do with this approach

NAMING CONVENTION: Tables in plural, Data Models in singular

![ER Diagram](erdiagram.JPG?raw=true "Title")

## Data Model

### DbContext

```csharp
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

```

### DbContextFactory

```csharp
public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
{
    public RestaurantDbContext CreateDbContext(string[]? args = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();
        optionsBuilder
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
            .UseSqlServer(configuration.GetConnectionString("Default"));
        //.UseSqlite("Data Source=blog.db");

        return new RestaurantDbContext(optionsBuilder.Options);
    }
}
```

### Dish

```csharp
public class Dish
{
    public int DishId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string? Notes { get; set; }
    public int? Stars { get; set; }

    // 1 dish can have 1+ ingredients
    public List<Ingredient> Ingredients { get; set; } = new();
}

public class DishEntityTypeConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        // Constraints
        builder.HasKey(x => x.DishId);
        builder.Property(x => x.Title).HasMaxLength(50);
        builder.Property(x => x.Notes).HasMaxLength(200);
        builder.HasCheckConstraint("Constraint_Stars_gt0_lt6", @$"[{nameof(Dish.Stars)}] > 0 AND [{nameof(Dish.Stars)}] < 6");

    }
}
```

### Ingredient

```csharp
public class Ingredient
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = String.Empty;

    // 1 Ingredient may be part of 0+ dish(es)
    public List<Dish> Dishes { get; set; } = new();
}

public class IngredientEntityTypeConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(x => x.IngredientId);
        builder.Property(x => x.Name).HasMaxLength(25);
    }
}
```
