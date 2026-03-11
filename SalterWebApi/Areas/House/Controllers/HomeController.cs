using HomeServiceHelper.IService;
using HomeServiceHelper.Models.DTO.ViewModels;
using HomeServiceHelper.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.House.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomService _homService;

        public HomeController(IHomService homeService)
        {
            _homService = homeService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _homService.GetAllHousesAsync();
            return Ok(results);
        }

        
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] HouseSearchDTO search)
        {
            var results = await _homService.SearchHousesAsync(search);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _homService.SerchHouseDetailAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的房間" });
            }

            return Ok(result);
        }

        
    }
}
