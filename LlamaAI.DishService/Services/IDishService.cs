using LlamaAI.DishService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LlamaAI.DishService.Services
{
    public interface IDishService
    {
        Task<IEnumerable<DishModel>> GetAllDishesAsync();
        Task<DishModel> GetDishByNameAsync(string name);
        Task<IEnumerable<DishModel>> GetDishesByAllergenAsync(string allergen);
        Task<IEnumerable<DishModel>> GetDishesByIngredientAsync(string ingredient);
        Task<IEnumerable<DishModel>> GetDishesWithinCalorieRangeAsync(double minCalories, double maxCalories);
        Task<MenuPlan> GenerateTwoWeekMenuPlanAsync();
        Task<MenuPlan> GenerateTwoWeekMenuPlanAsync(List<DishModel> dishes);
    }
}
