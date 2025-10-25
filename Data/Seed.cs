using Backend_Evolution.Models;

namespace Backend_Evolution.Data
{
    public static class Seed
    {
        public static async Task EnsureSeedAsync(FoodDbContext db)
        {
            // Nếu đã có dữ liệu → bỏ qua seed
            if (db.Dishes.Any() || db.Users.Any() || db.Categories.Any())
                return;

            // ===== USERS =====
            var users = new List<User>
            {
                new User { Username = "admin", PasswordHash = "123456", Email = "admin@evolution.ai", Role = "admin" },
                new User { Username = "chef", PasswordHash = "123456", Email = "chef@evolution.ai", Role = "user" },
                new User { Username = "tester", PasswordHash = "123456", Email = "tester@evolution.ai", Role = "user" }
            };
            db.Users.AddRange(users);

            // ===== CATEGORIES =====
            var categories = new List<Category>
            {
                new Category { Name = "Món Việt", Icon = "🍜", Color = "bg-orange-100 text-orange-600" },
                new Category { Name = "Món Á", Icon = "🍱", Color = "bg-red-100 text-red-600" },
                new Category { Name = "Món Âu", Icon = "🍝", Color = "bg-yellow-100 text-yellow-600" },
                new Category { Name = "Tráng miệng", Icon = "🍰", Color = "bg-pink-100 text-pink-600" },
                new Category { Name = "Đồ uống", Icon = "🧋", Color = "bg-purple-100 text-purple-600" },
                new Category { Name = "Ăn vặt", Icon = "🍿", Color = "bg-blue-100 text-blue-600" }
            };
            db.Categories.AddRange(categories);

            // ===== DISHES =====
            var dishes = new List<Dish>
            {
                new Dish
                {
                    Name = "Phở Bò Hà Nội",
                    Description = "Phở truyền thống với nước dùng trong, thịt bò tươi ngon.",
                    ImageUrl = "https://cdn.pastaxi-manager.onepas.vn/content/uploads/articles/01-Phuong-Mon%20ngon&congthuc/1.%20pho%20ha%20noi/canh-nau-pho-ha-noi-xua-mang-huong-vi-kinh-do-cua-80-nam-ve-truoc-1.jpg",
                    CategoryId = 1,
                    Difficulty = "Trung bình",
                    CookingTime = 30,
                    RatingAvg = 4.8
                },
                new Dish
                {
                    Name = "Bún Chả Hà Nội",
                    Description = "Bún với thịt nướng và nước mắm chua ngọt.",
                    ImageUrl = "https://i-giadinh.vnecdn.net/2023/04/16/Buoc-11-Thanh-pham-11-7068-1681636164.jpg",
                    CategoryId = 1,
                    Difficulty = "Dễ",
                    CookingTime = 45,
                    RatingAvg = 4.9
                },
                new Dish
                {
                    Name = "Cơm Tấm Sài Gòn",
                    Description = "Cơm tấm, sườn nướng và chả trứng thơm ngon.",
                    ImageUrl = "https://emdoi.vn/wp-content/uploads/2025/04/com-tam-ngon-quan-1-0.webp",
                    CategoryId = 1,
                    Difficulty = "Dễ",
                    CookingTime = 40,
                    RatingAvg = 4.7
                },
                new Dish
                {
                    Name = "Trà Sữa Trân Châu",
                    Description = "Đồ uống yêu thích với vị ngọt thanh và trân châu đen.",
                    ImageUrl = "https://xingfuvietnam.vn/wp-content/uploads/2023/02/xingfu-tra-sua-tran-chau-duong-den-2-FILEminimizer.jpg",
                    CategoryId = 5,
                    Difficulty = "Dễ",
                    CookingTime = 15,
                    RatingAvg = 4.5
                }
            };
            db.Dishes.AddRange(dishes);
            await db.SaveChangesAsync();

            // ===== INGREDIENTS =====
            var ingredients = new List<Ingredient>
            {
                new Ingredient { DishId = 1, Name = "Bánh phở", Unit = "g", Quantity = 200 },
                new Ingredient { DishId = 1, Name = "Thịt bò tái", Unit = "g", Quantity = 150 },
                new Ingredient { DishId = 1, Name = "Hành lá", Unit = "g", Quantity = 10 },
                new Ingredient { DishId = 2, Name = "Thịt heo ba chỉ", Unit = "g", Quantity = 200 },
                new Ingredient { DishId = 3, Name = "Cơm tấm", Unit = "bát", Quantity = 1 },
                new Ingredient { DishId = 4, Name = "Trân châu", Unit = "g", Quantity = 50 }
            };
            db.Ingredients.AddRange(ingredients);

            // ===== RECIPE STEPS =====
            var steps = new List<RecipeStep>
            {
                new RecipeStep { DishId = 1, Order = 1, Instruction = "Nấu nước dùng từ xương bò trong 3 tiếng." },
                new RecipeStep { DishId = 1, Order = 2, Instruction = "Thái thịt bò mỏng và trụng sơ." },
                new RecipeStep { DishId = 2, Order = 1, Instruction = "Nướng thịt ba chỉ ướp sẵn trên than." },
                new RecipeStep { DishId = 3, Order = 1, Instruction = "Nướng sườn, chiên trứng, dọn cơm." },
                new RecipeStep { DishId = 4, Order = 1, Instruction = "Ủ trà, thêm sữa và trân châu." }
            };
            db.RecipeSteps.AddRange(steps);

            await db.SaveChangesAsync();

            // ===== FAVORITES =====
            var favorites = new List<Favorite>
            {
                new Favorite { UserId = 2, DishId = 1 },
                new Favorite { UserId = 3, DishId = 2 }
            };
            db.Favorites.AddRange(favorites);

            // ===== REVIEWS =====
            var reviews = new List<Review>
            {
                new Review { UserId = 2, DishId = 1, Rating = 5, Comment = "Nước phở rất ngon, đúng vị Hà Nội." },
                new Review { UserId = 3, DishId = 2, Rating = 4, Comment = "Thịt nướng thơm, nước mắm vừa miệng." }
            };
            db.Reviews.AddRange(reviews);

            await db.SaveChangesAsync();
        }
    }
}
