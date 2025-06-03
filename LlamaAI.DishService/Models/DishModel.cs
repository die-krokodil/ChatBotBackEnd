using System.Collections.Generic;

namespace LlamaAI.DishService.Models
{
    public class DishModel
    {
        public string Name { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Additives { get; set; }
        public List<string> BigEightAllergens { get; set; }
        public NutritionInfo BigEightNutrition { get; set; }
    }

    public class NutritionInfo
    {
        public double CaloriesKcal { get; set; }
        public double ProteinG { get; set; }
        public double FatG { get; set; }
        public double CarbohydratesG { get; set; }
        public double SugarG { get; set; }
        public double SaltG { get; set; }
    }

    public class DishMenuData
    {
        public List<DishModel> Dishes { get; set; }
    }
}
