using System.Text.Json.Serialization;

namespace Backend_Evolution.Models;

public class RecipeStep
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Instruction { get; set; } = null!;
    public int DishId { get; set; }

    [JsonIgnore]
    public Dish? Dish { get; set; }
}
