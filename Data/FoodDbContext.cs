using Backend_Evolution.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Backend_Evolution.Data;

public class FoodDbContext : DbContext
{
    public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options) { }

    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();

    // ✅ thêm mới
    public DbSet<User> Users => Set<User>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Category> Categories { get; set; }


    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Dish>()
            .HasMany(d => d.Ingredients)
            .WithOne(i => i.Dish)
            .HasForeignKey(i => i.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Dish>()
            .HasMany(d => d.Steps)
            .WithOne(s => s.Dish)
            .HasForeignKey(s => s.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<RecipeStep>()
            .HasIndex(s => new { s.DishId, s.Order })
            .IsUnique();

        // ✅ quan hệ User–Favorite–Dish
        b.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Favorite>()
            .HasOne(f => f.Dish)
            .WithMany()
            .HasForeignKey(f => f.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ quan hệ Review–User–Dish
        b.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Review>()
            .HasOne(r => r.Dish)
            .WithMany(d => d.Reviews)
            .HasForeignKey(r => r.DishId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
