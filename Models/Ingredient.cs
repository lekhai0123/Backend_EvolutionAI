using System.Text.Json.Serialization;

namespace Backend_Evolution.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Unit { get; set; } = "g";
    public float Quantity { get; set; }
    public int DishId { get; set; }

    [JsonIgnore]
    public Dish? Dish { get; set; }
}
