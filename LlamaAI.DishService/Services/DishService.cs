using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LlamaAI.DishService.Models;

namespace LlamaAI.DishService.Services
{
    public class DishService : IDishService
    {
        private readonly string _jsonFilePath;
        private List<DishModel> _dishes;

        public DishService()
        {
            _jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "dishes.json");
        }

        private async Task LoadDishesAsync()
        {
            if (_dishes != null) return;

            if (!File.Exists(_jsonFilePath))
            {
                throw new FileNotFoundException("Dishes data file not found", _jsonFilePath);
            }

            var jsonString = await File.ReadAllTextAsync(_jsonFilePath);
            var menuData = JsonSerializer.Deserialize<DishMenuData>(jsonString);
            _dishes = menuData.Dishes;
        }

        public async Task<IEnumerable<DishModel>> GetAllDishesAsync()
        {
            await LoadDishesAsync();
            return _dishes;
        }

        public async Task<DishModel> GetDishByNameAsync(string name)
        {
            await LoadDishesAsync();
            return _dishes.FirstOrDefault(d => 
                d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<DishModel>> GetDishesByAllergenAsync(string allergen)
        {
            await LoadDishesAsync();
            return _dishes.Where(d => 
                d.BigEightAllergens.Any(a => 
                    a.Equals(allergen, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<DishModel>> GetDishesByIngredientAsync(string ingredient)
        {
            await LoadDishesAsync();
            return _dishes.Where(d => 
                d.Ingredients.Any(i => 
                    i.Equals(ingredient, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<DishModel>> GetDishesWithinCalorieRangeAsync(
            double minCalories, double maxCalories)
        {
            await LoadDishesAsync();
            return _dishes.Where(d => 
                d.BigEightNutrition.CaloriesKcal >= minCalories && 
                d.BigEightNutrition.CaloriesKcal <= maxCalories);
        }

        public async Task<MenuPlan> GenerateTwoWeekMenuPlanAsync()
        {
            await LoadDishesAsync();

            var random = new Random();
            var menuPlan = new MenuPlan { DailyMenus = new List<DailyMenu>() };

            for (int i = 0; i < 14; i++)
            {
                var dailyMenu = new DailyMenu
                {
                    Date = DateTime.Today.AddDays(i),
                    Dishes = _dishes.OrderBy(x => random.Next()).Take(3).ToList()
                };

                menuPlan.DailyMenus.Add(dailyMenu);
            }

            return menuPlan;
        }

        public async Task<MenuPlan> GenerateTwoWeekMenuPlanAsync(List<DishModel> dishes)
        {
            var random = new Random();
            var menuPlan = new MenuPlan { DailyMenus = new List<DailyMenu>() };

            for (int i = 0; i < 14; i++)
            {
                var dailyMenu = new DailyMenu
                {
                    Date = DateTime.Today.AddDays(i),
                    Dishes = dishes.OrderBy(x => random.Next()).Take(3).ToList()
                };

                menuPlan.DailyMenus.Add(dailyMenu);
            }

            return menuPlan;
        }
    }
}
