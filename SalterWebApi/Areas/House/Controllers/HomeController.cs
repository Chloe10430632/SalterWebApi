using CloudinaryDotNet.Actions;
using HomeServiceHelper.IService;
using HomeServiceHelper.Models.DTO.ViewModels;
using HomeServiceHelper.Models.DTO.ViewModels.Review;
using HomeServiceHelper.Service;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        //取得所有房型
        [HttpGet] 
        public async Task<IActionResult> GetAll()
        {
            var results = await _homService.GetAllHousesAsync();
            return Ok(results);
        }

        //透過篩選條件搜尋
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] HouseSearchDTO search)
        {
            var results = await _homService.SearchHousesAsync(search);
            return Ok(results);
        }

        //透過ID搜尋顯示房屋細節
        [HttpGet("{roomTypeId}")]
        public async Task<IActionResult> GetDetail(int roomTypeId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? currentUserId = int.TryParse(userIdStr, out int id) ? id : null;

            var result = await _homService.SerchHouseDetailAsync(roomTypeId, currentUserId);

            if (result == null)
            {
                return NotFound(new { message = $"找不到 ID 為 {id} 的房間" });
            }

            return Ok(result);
        }

        //取得所有城市名
        [HttpGet("cities")]
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

        //顯示前幾筆的房屋
        [HttpGet("top/{count}")]
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

        //新增房屋
        [HttpPost("create-full-house")]
        public async Task<IActionResult> CreateFullHouse([FromBody] HouseCreateDTO dto)
        {
            var result = await _homService.CreateFullHouseAsync(dto);
            if (result)
            {
                return Ok(new { message = "房子及相關資訊新增成功！" });
            }
            return StatusCode(500, "新增房子時發生錯誤");
        }

        //取得房屋設備API
        [HttpGet("amenities")] 
        public async Task<IActionResult> GetAmenities()
        {
            var data = await _homService.GetAllAmenitiesAsync();

            if (data == null) return NotFound("找不到任何設施資料");
            return Ok(data);
        }

        //更新房屋資料
        [HttpPut("update-full-house")]
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
            var result = await _homService.SearchHousesAsync(city, keyword, guests);

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

        
        [Authorize]
        [HttpPost("createBookingId")]
        public async Task<IActionResult> CreateBookingId([FromBody] CreateBookingDTO dto)
        {

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return Unauthorized(new { message = "請先登入" });

                dto.UserId = int.Parse(currentUserId);
                var bookingId = await _homService.CreateBookingAsync(dto);
                return Ok(new { BookingID = bookingId, Message = "訂單已建立，請進行付款" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "預約失敗", Error = ex.Message });
            }
        }

       
        [Authorize]
        [HttpGet("getMemberBookings")]
        public async Task<IActionResult> GetMemberBookings()
        {
            // 1. 抓 ID 
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return Unauthorized();

          
            var result = await _homService.GetMemberBookingsAsync(int.Parse(currentUserId));

            return Ok(result);
        }


        [Authorize]
        [HttpPost("cancelBooking/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            // 從 Token 拿到當前登入者 ID
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var success = await _homService.CancelBookingAsync(id, int.Parse(userIdStr));

            if (!success)
            {
                return BadRequest(new { message = "無法取消該訂單（可能狀態不符或訂單不存在）" });
            }

            return Ok(new { message = "訂單已成功取消" });
        }

        // 更新評論
        [Authorize]
        [HttpPut("updateReview")]
        public async Task<IActionResult> Update([FromBody] ReviewUpdateDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId)) return Unauthorized();
            dto.MemberId = currentUserId;

            var success = await _homService.UpdateReviewAsync(dto);
            if (!success)
            {
                // 這裡可以根據需求回傳更細的錯誤
                return BadRequest("更新失敗，請確認資料是否存在或是否有權限。");
            }
            return Ok(new { message = "評論已更新" });
        }

        // 刪除評論
        [Authorize]
        [HttpDelete("delete/{reviewId}")]
        public async Task<IActionResult> Delete(int reviewId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId)) return Unauthorized();
            

            var success = await _homService.DeleteReviewAsync(reviewId, currentUserId);
            if (!success)
            {
                return BadRequest("刪除失敗。");
            }
            return Ok(new { message = "評論已刪除" });
        }
    }
}

    


