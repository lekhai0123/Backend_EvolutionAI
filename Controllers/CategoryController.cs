using Microsoft.AspNetCore.Mvc;
using Backend_Evolution.Data;
using Backend_Evolution.Models;

namespace Backend_Evolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly FoodDbContext _context;
        public CategoryController(FoodDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Categories.ToList());
    }
}
