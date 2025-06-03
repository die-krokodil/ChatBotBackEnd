using System;
using System.Collections.Generic;

namespace LlamaAI.DishService.Models
{
    public class DailyMenu
    {
        public DateTime Date { get; set; }
        public List<DishModel> Dishes { get; set; }
    }

    public class MenuPlan
    {
        public List<DailyMenu> DailyMenus { get; set; }
    }
}
