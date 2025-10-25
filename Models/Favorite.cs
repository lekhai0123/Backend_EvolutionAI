using System.Text.Json.Serialization;

namespace Backend_Evolution.Models;

public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DishId { get; set; }

    [JsonIgnore]
    public User User { get; set; } = null!;

    [JsonIgnore]
    public Dish Dish { get; set; } = null!;
}
