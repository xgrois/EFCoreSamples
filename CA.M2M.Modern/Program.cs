// See https://aka.ms/new-console-template for more information

using CA.M2M.Modern.Data;
using CA.M2M.Modern.Entities;
using Microsoft.EntityFrameworkCore;

var factory = new RestaurantDbContextFactory();

// Suggestion before run:
//   Create migration files beforehand: "dotnet ef migrations add Init" 
//   You don't need to run "dotnet ef database update" if you do that with .MigrateAsync()

await DropDbAsync(factory);
await CreateOrUpdateDbAsync(factory);
await PopulateDbAsync(factory);
await QueryDbAsync(factory);


static async Task DropDbAsync(RestaurantDbContextFactory factory)
{
    using var db = factory.CreateDbContext();

    var result = await db.Database.EnsureDeletedAsync(); // Drop the database if it exists

    if (result) Console.WriteLine("Database already exists so it was deleted.");
    else Console.WriteLine("Database does not exist yet so it cannot be deleted.");
}

static async Task CreateOrUpdateDbAsync(RestaurantDbContextFactory factory)
{
    using var db = factory.CreateDbContext();

    // If migration files do not exist,
    //  - it will still create the DB but just with a single EFMigrationsHistory Table
    //  - you can always run "dotnet ef migrations add Init" later
    //      - if you did it, MigrateAsync() will update the existing "dummy" DB with your Tables 
    // If migration files exist,
    //  - it will apply migrations to update DB model(s)
    //      - this means the DB is created with all your Tables from scratch
    //      - or do nothing if that migration file was already applied before
    await db.Database.MigrateAsync();

    Console.WriteLine("Migrations applied.");
}

static async Task PopulateDbAsync(RestaurantDbContextFactory factory)
{
    using var db = factory.CreateDbContext();

    // In a manual Many-to-Many relationship (Dishes-Ingredients)
    // we add first data to one of the tables
    Ingredient pasta, tomato, rice, veggies, chicken;
    await db.AddRangeAsync(new[]
    {
        pasta = new Ingredient{ Name = "Pasta" },
        tomato = new Ingredient{ Name = "Tomato" },
        rice = new Ingredient{ Name = "Rice" },
        veggies = new Ingredient{ Name = "Veggies" },
        chicken = new Ingredient{ Name = "Chicken" }
    });

    await db.SaveChangesAsync();

    // Then, we add data to the another table, including the relation in the join table
    // E.g., here we populate the "DishesToIngredients" join table
    Dish creamyTomatoPasta, bakedHoneyMustardChicken, fishAndChips;
    await db.AddRangeAsync(new[]
    {
        creamyTomatoPasta = new Dish
        {
            Title = "Creamy Tomato Pasta",
            Notes = "This isn't the most orthodox pasta recipe, but it is a good one.",
            Stars = 3,
            Ingredients = new() { pasta, tomato }
        },
        bakedHoneyMustardChicken = new Dish
        {
            Title = "Baked Honey Mustard Chicken",
            Notes = "If you're not used to using the microwave when cooking, this is a great opportunity to make this appliance your new ally.",
            Stars = 2,
            Ingredients = new() { chicken }
        },
        fishAndChips = new Dish
        {
            Title = "Fish and Chips",
            Notes = "Popular hot dish consisting of fried fish in crispy batter, served with chips",
            Stars = 3,
        }

    });

    await db.SaveChangesAsync();

    // Now, we have Dishes, DishesToIngredients and Ingredients tables properly populated
    // Note that we haven't inserted anything directly in the join table "DishesToIngredients"

}


static async Task QueryDbAsync(RestaurantDbContextFactory factory)
{
    using var db = factory.CreateDbContext();

    // Cannot query from join Table (DishIngredient)
    // since it is hidden in the data model in this modern approach


    // Get dishes data, including ingredients (internally, it will use the join table,
    // hidden in this modern approach
    // Dishes -> hidden(DishesIngredients) -> Ingredients
    var dishes = await db.Dishes
        .Include(i => i.Ingredients)
        .ToArrayAsync();

    foreach (var dish in dishes)
    {
        if (dish.Ingredients.Count == 0)
            Console.WriteLine($"Dish {dish.Title} has no ingredients yet");
        else Console.WriteLine($"Dish {dish.Title} has {String.Join(',', dish.Ingredients.Select(i => i.Name))}");
    }


}


/*
var dish1 = new Dish
{
    Title = "Creamy Tomato Pasta",
    Notes = "This isn't the most orthodox pasta recipe, but it is a good one.",
    Stars = 3
};

db.Dishes.Add(dish1); // this is not added inmediately
await db.SaveChangesAsync(); // this makes th db operations

dish1.Stars = 4;
await db.SaveChangesAsync();

*/

/*
var pastaDishes = await db.Dishes
    .Where(d => d.Title.Contains("Pasta")) // LINQ -> SQL
    .ToListAsync(); // This is required to make the actual query

foreach (var dish in pastaDishes)
{
    Console.WriteLine($"Pasta dish: {dish.Title}");
}
*/

/*
//await EntityStates(factory);
await ChangeTracking(factory);

static async Task EntityStates(RestaurantDbContextFactory factory)
{
    using var db = factory.CreateDbContext();

    var dish = new Dish
    {
        Title = "Pot Roast",
        Notes = "This flaky and tender beef roast cooks in the oven for 3 hours.",
        Stars = 4
    };


    Console.WriteLine($"1 Dish state: {db.Entry(dish).State}");

    dish.Stars = 1;
    Console.WriteLine($"2 Dish state: {db.Entry(dish).State}");

    db.Dishes.Add(dish);
    Console.WriteLine($"3 Dish state: {db.Entry(dish).State}");

    await db.SaveChangesAsync();
    Console.WriteLine($"4 Dish state: {db.Entry(dish).State}");

    dish.Stars = 4;
    Console.WriteLine($"5 Dish state: {db.Entry(dish).State}");

    await db.SaveChangesAsync();
    Console.WriteLine($"6 Dish state: {db.Entry(dish).State}");

    db.Dishes.Remove(dish);
    Console.WriteLine($"7 Dish state: {db.Entry(dish).State}");

    await db.SaveChangesAsync();
    Console.WriteLine($"8 Dish state: {db.Entry(dish).State}");
}

static async Task ChangeTracking(RestaurantDbContextFactory factory)
{
    using var db = factory.CreateDbContext();

    var dish = new Dish
    {
        Title = "Pot Roast",
        Notes = "This flaky and tender beef roast cooks in the oven for 3 hours.",
        Stars = 4
    };

    db.Dishes.Add(dish);
    await db.SaveChangesAsync();

    dish.Stars = 2;
}
*/