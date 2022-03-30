using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CA.M2M.Classic.Entities;
public class Ingredient
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = String.Empty;

    // 1 Ingredient may be part of 0+ dish(es)
    public List<DishToIngredient> DishesToIngredients { get; set; } = new();
}

public class IngredientEntityTypeConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(x => x.IngredientId);
        builder.Property(x => x.Name).HasMaxLength(25);
    }
}
