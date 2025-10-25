using System.Text.Json.Serialization;

namespace Backend_Evolution.Models;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DishId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public User User { get; set; } = null!;

    [JsonIgnore]
    public Dish Dish { get; set; } = null!;
}
