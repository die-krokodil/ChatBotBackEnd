using LlamaAI.DishService.Models;
using LlamaAI.DishService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LlamaAI.DishService.Tests
{
    public class DishServiceTests
    {
        private readonly DishService _dishService;

        public DishServiceTests()
        {
            _dishService = new DishService();
        }

        [Fact]
        public async Task GetAllDishesAsync_ShouldReturnAllDishes()
        {
            var dishes = await _dishService.GetAllDishesAsync();
            Assert.NotNull(dishes);
            Assert.True(dishes.Any());
        }

        [Fact]
        public async Task GetDishByNameAsync_ShouldReturnCorrectDish()
        {
            var dish = await _dishService.GetDishByNameAsync("Classic Caesar Salad");
            Assert.NotNull(dish);
            Assert.Equal("Classic Caesar Salad", dish.Name);
        }

        [Fact]
        public async Task GetDishesByAllergenAsync_ShouldReturnDishesWithAllergen()
        {
            var dishes = await _dishService.GetDishesByAllergenAsync("Milk");
            Assert.NotNull(dishes);
            Assert.True(dishes.Any());
            Assert.Contains(dishes, d => d.BigEightAllergens.Contains("Milk"));
        }

        [Fact]
        public async Task GetDishesByIngredientAsync_ShouldReturnDishesWithIngredient()
        {
            var dishes = await _dishService.GetDishesByIngredientAsync("Chicken Breast");
            Assert.NotNull(dishes);
            Assert.True(dishes.Any());
            Assert.Contains(dishes, d => d.Ingredients.Contains("Chicken Breast"));
        }

        [Fact]
        public async Task GetDishesWithinCalorieRangeAsync_ShouldReturnDishesWithinRange()
        {
            var dishes = await _dishService.GetDishesWithinCalorieRangeAsync(100, 200);
            Assert.NotNull(dishes);
            Assert.True(dishes.Any());
            Assert.All(dishes, d => Assert.InRange(d.BigEightNutrition.CaloriesKcal, 100, 200));
        }

        [Fact]
        public async Task GenerateTwoWeekMenuPlanAsync_ShouldGenerateMenuForFourteenDays()
        {
            // Arrange
            var dishService = new DishService();

            // Act
            var menuPlan = await dishService.GenerateTwoWeekMenuPlanAsync();

            // Assert
            Assert.NotNull(menuPlan);
            Assert.Equal(14, menuPlan.DailyMenus.Count);

            foreach (var dailyMenu in menuPlan.DailyMenus)
            {
                Assert.NotNull(dailyMenu);
                Assert.Equal(3, dailyMenu.Dishes.Count);
                Assert.True(dailyMenu.Date >= DateTime.Today && dailyMenu.Date < DateTime.Today.AddDays(14));
            }
        }
    }
}
