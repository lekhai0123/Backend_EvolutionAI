using Backend_Evolution.Data;
using Backend_Evolution.DTOs;
using Backend_Evolution.Models;
using Backend_Evolution.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class DishesController : ControllerBase
{
    private readonly FoodDbContext _db;
    public DishesController(FoodDbContext db) { _db = db; }

    // -------------------------------
    // LẤY TẤT CẢ MÓN ĂN
    // -------------------------------
    // -------------------------------
    // LẤY TẤT CẢ MÓN ĂN
    // -------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dish>>> GetAll()
    {
        var dishes = await _db.Dishes
            .Include(d => d.Ingredients)
            .Include(d => d.Steps.OrderBy(s => s.Order))
            .ToListAsync();

        foreach (var d in dishes)
            d.RatingAvg = await _db.Favorites.CountAsync(f => f.DishId == d.Id);

        return dishes;
    }

    // -------------------------------
    // LẤY 1 MÓN ĂN CHI TIẾT (CÓ ISFAVORITE)
    // -------------------------------
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetOne(int id)
    {
        var dish = await _db.Dishes
            .Include(d => d.Ingredients)
            .Include(d => d.Steps.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dish == null) return NotFound();

        // ✅ tổng số yêu thích
        dish.RatingAvg = await _db.Favorites.CountAsync(f => f.DishId == id);

        // ✅ kiểm tra người dùng hiện tại
        var userName = User.Identity?.Name;
        bool isFavorite = false;

        if (!string.IsNullOrEmpty(userName))
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userName);
            if (user != null)
                isFavorite = await _db.Favorites.AnyAsync(f => f.UserId == user.Id && f.DishId == id);
        }

        return new
        {
            dish.Id,
            dish.Name,
            dish.ImageUrl,
            dish.Description,
            dish.Difficulty,
            dish.CookingTime,
            dish.CategoryId,
            dish.Ingredients,
            dish.Steps,
            LikesCount = dish.RatingAvg,
            IsFavorite = isFavorite
        };
    }
    // -------------------------------
    // TẠO MÓN ĂN MỚI
    // -------------------------------
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Dish>> Create([FromForm] DishCreateDto dto, [FromServices] CloudinaryService cloud)
    {
        if (dto.Image == null || dto.Image.Length == 0)
            return BadRequest("Ảnh không hợp lệ");

        var imageUrl = await cloud.UploadAsync(dto.Image);

        var dish = new Dish
        {
            Name = dto.Name,
            Description = dto.Description,
            Difficulty = dto.Difficulty,
            CookingTime = dto.CookingTime,
            ImageUrl = imageUrl,
            CategoryId = dto.CategoryId,
            Ingredients = dto.Ingredients?
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(i => new Ingredient { Name = i.Trim() })
                .ToList() ?? new(),
            Steps = dto.Steps?
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select((s, i) => new RecipeStep { Order = i + 1, Instruction = s.Trim() })
                .ToList() ?? new()
        };

        _db.Dishes.Add(dish);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOne), new { id = dish.Id }, dish);
    }

    // -------------------------------
    // CẬP NHẬT MÓN ĂN
    // -------------------------------
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Dish dto)
    {
        if (id != dto.Id) return BadRequest();
        _db.Entry(dto).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // -------------------------------
    // XOÁ MÓN ĂN
    // -------------------------------
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dish = await _db.Dishes.FindAsync(id);
        if (dish is null) return NotFound();
        _db.Dishes.Remove(dish);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // -------------------------------
    // TÌM KIẾM CÓ PHÂN TRANG
    // -------------------------------
    [HttpGet("search")]
    public async Task<ActionResult<object>> Search(
    string? q,
    int? categoryId,
    int page = 1,
    int size = 10,
    string sort = "name")
    {
        var query = _db.Dishes
            .Include(d => d.Ingredients)
            .Include(d => d.Steps)
            .Include(d => d.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(d => d.Name.Contains(q) || (d.Description != null && d.Description.Contains(q)));

        if (categoryId.HasValue)
            query = query.Where(d => d.CategoryId == categoryId.Value);

        query = sort.ToLower() switch
        {
            "category" => query.OrderBy(d => d.Category!.Name),
            "id" => query.OrderBy(d => d.Id),
            _ => query.OrderBy(d => d.Name)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        foreach (var d in items)
            d.RatingAvg = await _db.Favorites.CountAsync(f => f.DishId == d.Id);

        return new
        {
            total,
            page,
            size,
            totalPages = (int)Math.Ceiling(total / (double)size),
            data = items
        };
    }
    // -------------------------------
    // UPLOAD ẢNH
    // -------------------------------
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromServices] CloudinaryService cloud)
    {
        if (file == null || file.Length == 0) return BadRequest("No file");
        var url = await cloud.UploadAsync(file);
        return Ok(new { imageUrl = url });
    }
}
