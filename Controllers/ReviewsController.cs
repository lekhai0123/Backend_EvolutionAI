using Backend_Evolution.Data;
using Backend_Evolution.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly FoodDbContext _db;
    public ReviewsController(FoodDbContext db) { _db = db; }

    [HttpPost("{dishId}")]
    public async Task<IActionResult> PostReview(int dishId, [FromBody] Review review)
    {
        var userName = User.Identity!.Name!;
        var user = await _db.Users.FirstAsync(u => u.Username == userName);
        review.UserId = user.Id;
        review.DishId = dishId;

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{dishId}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews(int dishId)
    {
        return await _db.Reviews
            .Where(r => r.DishId == dishId)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
