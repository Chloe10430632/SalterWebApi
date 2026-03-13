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

        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            return Ok(await _homService.GetAllCityAsync());
        }

        [HttpGet("top/{count}")]
        public async Task<IActionResult> GetTop(int count = 6)
        {
            return Ok(await _homService.GetTopRoomsAsync(count));
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDTO dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest("評分必須在 1 到 5 之間");
            }

            var result = await _homService.AddReviewAsync(dto);

            if (result)
            {
                return Ok(new { message = "評論新增成功！" });
            }

            return StatusCode(500, "新增評論時發生錯誤");
        }
    }
}
