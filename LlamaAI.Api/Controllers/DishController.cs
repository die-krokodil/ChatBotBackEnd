using LlamaAI.DishService.Models;
using LlamaAI.DishService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LlamaAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpGet("dishes")]
        public async Task<ActionResult<IEnumerable<DishModel>>> GetAllDishes()
        {
            var dishes = await _dishService.GetAllDishesAsync();
            return Ok(dishes);
        }

        [HttpGet("menu-plan")]
        public async Task<ActionResult<MenuPlan>> GenerateTwoWeekMenuPlan()
        {
            var menuPlan = await _dishService.GenerateTwoWeekMenuPlanAsync();
            return Ok(menuPlan);
        }
    }
}
