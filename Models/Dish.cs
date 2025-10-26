using Backend_Evolution.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Dish
{
    public int Id { get; set; }
    public int? UserId { get; set; }  
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    // Quan hệ Category chuẩn
    public int CategoryId { get; set; }   // FK
    public Category? Category { get; set; }

    public string Difficulty { get; set; } = "Medium";
    public int CookingTime { get; set; } = 30;

    [NotMapped]
    public double RatingAvg { get; set; }

    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<RecipeStep> Steps { get; set; } = new List<RecipeStep>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
