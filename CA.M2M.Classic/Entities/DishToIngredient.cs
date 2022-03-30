using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CA.M2M.Classic.Entities;

/// <summary>
/// Join Table
/// </summary>
public class DishToIngredient
{
    // FK from Dishes
    public int DishId { get; set; }
    public Dish Dish { get; set; } = default!;

    // FK from Ingredients
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = default!;
}

public class DishToIngredientEntityTypeConfiguration : IEntityTypeConfiguration<DishToIngredient>
{
    public void Configure(EntityTypeBuilder<DishToIngredient> builder)
    {
        builder.ToTable("DishesToIngredients");

        builder.HasKey(x => new { x.DishId, x.IngredientId });

        builder.HasOne(dti => dti.Dish)
            .WithMany(d => d.DishesToIngredients)
            .HasForeignKey(dti => dti.DishId);

        builder.HasOne(dti => dti.Ingredient)
            .WithMany(i => i.DishesToIngredients)
            .HasForeignKey(dti => dti.IngredientId);

    }
}
