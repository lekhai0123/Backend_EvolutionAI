using Backend_Evolution.Data;
using Backend_Evolution.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly FoodDbContext _db;
    public FavoritesController(FoodDbContext db) { _db = db; }

    // ✅ Toggle yêu thích / bỏ yêu thích
    [HttpPost("{dishId}")]
    public async Task<IActionResult> ToggleFavorite(int dishId)
    {
        var userName = User.Identity!.Name!;
        var user = await _db.Users.FirstAsync(u => u.Username == userName);

        var fav = await _db.Favorites
            .FirstOrDefaultAsync(f => f.DishId == dishId && f.UserId == user.Id);

        if (fav != null)
        {
            // ❌ Nếu đã yêu thích → xoá
            _db.Favorites.Remove(fav);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Đã bỏ yêu thích", isFavorite = false });
        }

        // ❤️ Nếu chưa có → thêm mới
        _db.Favorites.Add(new Favorite { DishId = dishId, UserId = user.Id });
        await _db.SaveChangesAsync();
        return Ok(new { message = "Đã thêm yêu thích", isFavorite = true });
    }

    // ✅ Lấy danh sách yêu thích
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dish>>> GetFavorites()
    {
        var userName = User.Identity!.Name!;
        var user = await _db.Users.FirstAsync(u => u.Username == userName);

        var dishes = await _db.Favorites
            .Where(f => f.UserId == user.Id)
            .Include(f => f.Dish)
            .ThenInclude(d => d.Ingredients)
            .Select(f => f.Dish!)
            .ToListAsync();

        return dishes;
    }
}
