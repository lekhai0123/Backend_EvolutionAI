namespace Backend_Evolution.DTOs
{
    public class DishCreateDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }       // 🆕 Mô tả món ăn
        public string Difficulty { get; set; } = "Medium"; // 🆕 Độ khó
        public int CookingTime { get; set; } = 30;     // 🆕 Phút nấu
        public string Ingredients { get; set; } = "";
        public string Steps { get; set; } = "";
        public IFormFile? Image { get; set; }
        public int CategoryId { get; set; }
    }
}
