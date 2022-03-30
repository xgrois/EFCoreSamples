using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CA.M2M.Modern.Entities;

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

