using System.ComponentModel.DataAnnotations;

namespace Backend_Evolution.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = "user"; // "user" hoặc "admin"
    public string Email { get; set; } = "";
}
