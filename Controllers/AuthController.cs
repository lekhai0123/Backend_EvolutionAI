using Backend_Evolution.Data;
using Backend_Evolution.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FoodDbContext _db;
    private readonly IConfiguration _cfg;
    public AuthController(FoodDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User dto)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest("Username đã tồn tại");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok("Đăng ký thành công");
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Sai tài khoản hoặc mật khẩu");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token) });
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<object>> GetProfile()
    {
        var username = User.Identity?.Name;
        if (username == null) return Unauthorized();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return NotFound();

        var postCount = await _db.Dishes.CountAsync(d => d.UserId == user.Id);

        // 1) Lượt thích user đã bấm
        var likesGiven = await _db.Favorites.CountAsync(f => f.UserId == user.Id);

        // 2) Lượt thích nhận được trên tất cả món user đăng
        var likesReceived = await (from f in _db.Favorites
                                   join d in _db.Dishes on f.DishId equals d.Id
                                   where d.UserId == user.Id
                                   select f.Id).CountAsync();

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            PostCount = postCount,
            LikesGiven = likesGiven,
            LikesReceived = likesReceived
        });
    }
}

