using HomeServiceHelper.IService;
using HomeServiceHelper.Models.DTO.ViewModels;
using HomeServiceHelper.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.House.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("房屋租借")]
    public class HomeController : ControllerBase
    {
        private readonly IHomService _homService;

        public HomeController(IHomService homeService)
        {
            _homService = homeService;
        }


        [HttpGet] //取得所有房型
        public async Task<IActionResult> GetAll()
        {
            var results = await _homService.GetAllHousesAsync();
            return Ok(results);
        }


        [HttpPost("search")]//透過篩選條件搜尋
        public async Task<IActionResult> Search([FromBody] HouseSearchDTO search)
        {
            var results = await _homService.SearchHousesAsync(search);
            return Ok(results);
        }

        [HttpGet("{id}")]//透過ID搜尋顯示房屋細節
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _homService.SerchHouseDetailAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的房間" });
            }

            return Ok(result);
        }

        [HttpGet("cities")]//取得所有城市名
        public async Task<IActionResult> GetCities()
        {
            return Ok(await _homService.GetAllCityAsync());
        }

        [HttpGet("city-groups")]
        public async Task<ActionResult<List<CityGroupDTO>>> GetCityGroups(string? city)
        {
            // 這裡要把 city 傳給 Service
            var result = await _homService.GetCityGroupPreviewsAsync(city);
            return Ok(result);
        }


        [HttpGet("top/{count}")] //顯示前幾筆的房屋
        public async Task<IActionResult> GetTop(int count = 6)
        {
            return Ok(await _homService.GetTopRoomsAsync(count));
        }

        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDTO dto)
        {
            // 1. 檢查評分範圍
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest("評分必須在 1 到 5 之間");
            }

            //  安全檢查：自動抓取該用戶「真實且可用」的 BookingId
            var validBookingId = await _homService.GetAvailableBookingIdAsync(dto.MemberId, dto.RoomTypeId);

            if (validBookingId == null)
            {
                throw new ArgumentException("您沒有可評價的訂單，或是已經評價過了喔！");
            }

            // 3. 把抓到的真實 ID 塞進 DTO
            dto.BookingId = validBookingId.Value;

            // 4. 執行新增
            var result = await _homService.AddReviewAsync(dto);

            if (result)
            {
                return Ok(new { message = "評論新增成功！", bookingId = dto.BookingId });
            }

            return StatusCode(500, "新增評論時發生錯誤");
        }

        [HttpPost("create-full-house")] //新增房屋
        public async Task<IActionResult> CreateFullHouse([FromBody] HouseCreateDTO dto)
        {
            var result = await _homService.CreateFullHouseAsync(dto);
            if (result)
            {
                return Ok(new { message = "房子及相關資訊新增成功！" });
            }
            return StatusCode(500, "新增房子時發生錯誤");
        }

        [HttpGet("amenities")] //取得房屋設備API
        public async Task<IActionResult> GetAmenities()
        {
            var data = await _homService.GetAllAmenitiesAsync();

            if(data == null) return NotFound("找不到任何設施資料");
            return Ok(data);
        }

        [HttpPut("update-full-house")]//更新房屋資料
        public async Task<IActionResult> UpdateFullHouse([FromBody] HouseUpdateDTO dto)
        {
            // 基本驗證
            if (dto == null || dto.RoomTypeId <= 0)
            {
                return BadRequest(new { message = "無效的房源資料或 ID" });
            }

            // 呼叫 Service 執行「全功能更新」（包含地址、價格、圖片、設備）
            var success = await _homService.UpdateFullHouseAsync(dto);

            if (success)
            {
                return Ok(new { message = "房源資料與圖片已全面更新成功！" });
            }
            else
            {
                return NotFound(new { message = "更新失敗，找不到該房源或系統異常" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<HousePreviewDTO>>> Search(string? city, string? keyword, int? guests)
        {
            // 呼叫你剛才寫好的 Service 方法
            var result = await _homService.SearchHousesAsync(city, keyword,guests);

            if (result == null || !result.Any())
            {
                return Ok(new List<HousePreviewDTO>()); // 回傳空陣列，不要回傳 404，這樣前端比較好處理
            }

            return Ok(result);
        }

        [HttpGet("select")]
        public async Task<IActionResult> GetAvailableHouses([FromQuery] HouseSearchDTO searchCriteria)
        {
            // 呼叫業務邏輯層
            var results = await _homService.GetSearchHousesAsync(searchCriteria);
            return Ok(results);
        }
    }
}

