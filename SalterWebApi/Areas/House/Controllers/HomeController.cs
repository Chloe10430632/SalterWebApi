using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using HomeServiceHelper.IService;
using HomeServiceHelper.Models.DTO.ViewModels;
using HomeServiceHelper.Models.DTO.ViewModels.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ISECPay _sECpay;

        public HomeController(IHomService homeService, ISECPay sECpay)
        {
            _homService = homeService;
            _sECpay = sECpay;
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

        // 取得分組預覽，支援城市篩選
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

        //新增評論
        [HttpPost("reviews")]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateDTO dto)
        {
            try
            {
                // 建議在這裡也檢查一次權限，防止被用 Postman 暴力攻擊
                var createdReview = await _homService.AddReviewAsync(dto);

                return Ok(new
                {
                    message = "評論新增成功！",
                    reviewId = createdReview.ReviewId,
                    userId = createdReview.UserId 
                });
            }
            catch (Exception ex)
            {
                // 這裡會抓到 Service 拋出的 "找不到符合資格的訂單" 或 "資料庫儲存失敗"
                return BadRequest(new { message = ex.Message });
            }
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


        // 進階搜尋（包含日期、價格區間等）
        [HttpGet("select")]
        public async Task<IActionResult> GetAvailableHouses([FromQuery] HouseSearchDTO searchCriteria)
        {
            // 呼叫業務邏輯層
            var results = await _homService.GetSearchHousesAsync(searchCriteria);
            return Ok(results);
        }


        // 建立預約訂單 ID 
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

        // 取得自己的預約訂單列表
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

        // 取消預約訂單
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

        [Authorize]
        [HttpGet("CheckPermission/{userId}/{roomTypeId}")]
        public async Task<IActionResult> CheckPermission(int userId, int roomTypeId)
        {
            // 1. 呼叫 Service 取得 Response 物件
            var permissionResponse = await _homService.CheckReviewPermissionAsync(userId, roomTypeId);

            return Ok(new { canReview = permissionResponse.CanReview });
        }


        //交易
        [Authorize]
        [HttpPost("PayBooking/{bookingId}")]
        public async Task<IActionResult> PayBooking(int bookingId)
        {
            // 從 Token 抓出目前的 UserId
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { message = "請重新登入" });

            try
            {
                // 建立或取得 TransactionId
                int transactionId = await _homService.CreateTransactionForBooking(bookingId, userId);

                // 準備API 需要的 DTO
                var payRequest = new DTransacRequest
                {
                    TransactionId = transactionId,
                    ItemName = "房源預約費用",
                    Description = $"預約編號 #{bookingId} 之付款單",
                    // BaseUrl 可依需求傳入，或在 Service 內部由 Config 決定
                };

                // 呼叫 GetPaymentForm 產生 HTML 表單
                var result = await _sECpay.GetPaymentForm(payRequest);

                if (!result.IsSuccess)
                    return BadRequest(result);

                // 5. 回傳 HTML 給前端
                return Content(result.Data, "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        //接收綠界結果用的API
        [HttpPost("UpdateTransacForm")]
        [AllowAnonymous]
        //[Consumes("application/x-www-form-urlencoded")] // 綠界是用表單格式傳送
        public async Task<string> UpdateTransacForm([FromForm] Dictionary<string, string> data)
        {
            // 呼叫 Service 裡的 UpdateTransacForm 邏輯 (改狀態、改 DB)
            bool isSuccess = await _sECpay.UpdateTransacForm(data);

            if (isSuccess)
            {
                // 綠界收到這個字串才代表這筆通知處理完成
                return "1|OK";
            }
            else
            {
                return "0|Error";
            }
        }
    }
}




