using Backend_Evolution.Data;
using Backend_Evolution.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_Evolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly FoodDbContext _db;

        public UserController(FoodDbContext db)
        {
            _db = db;
        }

        // ===========================
        // GET /api/user
        // Lấy danh sách tất cả user (trừ admin hiện tại nếu muốn)
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool excludeSelf = false)
        {
            var q = _db.Users.AsQueryable();

            if (excludeSelf)
            {
                var currentUser = User.Identity?.Name;
                if (!string.IsNullOrEmpty(currentUser))
                    q = q.Where(u => u.Username != currentUser);
            }

            var users = await q
                .Select(u => new { u.Id, u.Username, u.Email, u.Role })
                .ToListAsync();

            return Ok(users);
        }
        
        // ===========================
        // GET /api/user/{id}
        // Lấy thông tin chi tiết user
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _db.Users
                .Select(u => new { u.Id, u.Username, u.Email, u.Role })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound("Không tìm thấy người dùng");
            return Ok(user);
        }

        // ===========================
        // POST /api/user
        // Tạo mới user (admin thêm thủ công)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Tên đăng nhập đã tồn tại");

            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email đã được sử dụng");

            var newUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role ?? "user"
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                newUser.Id,
                newUser.Username,
                newUser.Email,
                newUser.Role
            });
        }

        // ===========================
        // PUT /api/user/{id}/role
        // Đổi role admin/user
        // ===========================
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ToggleRole(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            user.Role = user.Role == "admin" ? "user" : "admin";
            await _db.SaveChangesAsync();

            return Ok(new { user.Id, user.Username, user.Role });
        }

        // ===========================
        // DELETE /api/user/{id}
        // Xoá user
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok("Đã xoá người dùng");
        }
    }

    // ===========================
    // DTOs dùng cho UserController
    // ===========================
    public class CreateUserDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = "";
        public string Password { get; set; } = null!;
        public string? Role { get; set; } = "user";
    }

}
