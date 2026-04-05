using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ExpServiceHelper.Service;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using TripServiceHelper.Models.DTOs;
using static ExpServiceHelper.DTO.DCoachFav;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalterWebApi.Areas.Experience
{
    [Area("Exp")]
    [Route("api/[area]/[controller]")] // 路由 api/Forum/Posts
    [ApiController]
    [Tags("行程裝備預約")] // Scalar 會用這個名字當分類標題。
    public class ExpController : ControllerBase
    {
        #region 

        #endregion
        #region DI
        private readonly ISCoachIndex _sCoachIndex;
        private readonly ISCoachMethods _sCoachMethods;
        private readonly ISECPay _sECPay;
        public ExpController(ISCoachIndex sCoachIndex, ISCoachMethods sCoachMethods, ISECPay sECPay)
        {
            _sCoachIndex = sCoachIndex;
            _sCoachMethods = sCoachMethods;
            _sECPay = sECPay;
        }
        #endregion
        #region ~~入口~~
        #region 排序
        #region 熱門排序 
        [HttpGet("PopRank")]
        public async Task<IActionResult> PopRank(int page = 1, int pageSize = 6)
        {
            var result = await _sCoachMethods.CoachPopular(page, pageSize);
            return Ok(new { IsSuccess = true, data = result });
        }
        #endregion
        #region 最新排序 
        [HttpGet("NewRank")]
        public async Task<IActionResult> NewRank(int page = 1, int pageSize = 6)
        {
            var result = await _sCoachMethods.GetCoachNewest(page, pageSize);
            return Ok(new { IsSuccess = true, data = result });
        }
        #endregion

        #endregion
        #region 搜尋

        #region~~搜尋-名字~~
        [HttpGet("NameSearch")]
        public async Task<IActionResult> NameSearch([FromQuery] string key)
        {
            try
            {
                var result = await _sCoachMethods.GetCoachName(key);
                return Ok(new { IsSuccess = true, data = result });
            }
            catch (Exception ex)
            {
                BadRequest(new { message = ex.Message });
            }
            return BadRequest(new { message = "請檢查資料是否正確" });
        }
        #endregion

        #region~~搜尋-專業~~
        [HttpGet("SpeSearch")]
        public async Task<IActionResult> SpeSearch([FromQuery] string key)
        {
            try
            {
                var result = await _sCoachMethods.GetCoachSpecial(key);
                return Ok(new { IsSuccess = true, data = result });
            }
            catch (Exception ex) { BadRequest(new { message = ex.Message }); }
            return BadRequest(new { message = "請檢查資料是否正確" });
        }
        #endregion

        #region~~搜尋-地區~~
        [HttpGet("DistSearch")]
        public async Task<IActionResult> DistSearch([FromQuery] string key)
        {
            try
            {
                var result = await _sCoachMethods.GetCoachDist(key);
                return Ok(new { IsSuccess = true, data = result });
            }
            catch (Exception ex) { BadRequest(new { message = ex.Message }); }
            return BadRequest(new { message = "請檢查資料是否正確" });
        }

        #endregion

        #endregion
        #endregion

        #region~~教練~~
        #region 申請加入教練(新增) 
        [Authorize]
        [HttpPost("BecomeCoach")]
        public async Task<IActionResult> BecomeCoach([FromForm] DCoachEdit dto) //因為有圖片 前端傳送資料時不能用 JSON，必須使用 multipart/form-data
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }
            try {
                if (int.TryParse(userIdStr, out int currentUserId)) {
                    var result = await _sCoachMethods.CreateCoach(dto, currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex){
                return Ok(new DAPIResponse<string>{
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
                return BadRequest();
        }

        #endregion

        #region 編輯自介 
        [Authorize]
        [HttpPut("EditCoach/{coachId}")] //TODO 測試是不是拿掉陸游上的ID 申請就正常了
        public async Task<IActionResult> EditThisCoach([FromForm] DCoachEdit dto, int coachId)
        {
            // 1. 抓取 JWS 裡面的 UserId
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

            try
            {
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var result = await _sCoachMethods.EditCoachInfo(dto, currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest("系統忙碌中");

        }
        #endregion

        #region 抓自己
        [Authorize]
        [HttpGet("MyInfo")]
        public async Task<IActionResult> MyInfo()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.MyCoachInfo(currentUserId);
                return Ok(new { IsSuccess = true, data = result });
            }
            return BadRequest("你是誰?");
        }
        #endregion

        #region 教練自介
        [HttpGet("Info/{coachId}")]
        public async Task<IActionResult> CoachInfo(int coachId)
        {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            var result = await _sCoachMethods.ThisCoachInfo(coachId);
            return Ok(new { isSuccess = true, data = result });

        }
        #endregion

        #region 系統推薦
        [HttpGet("Recommand/{id}")]
        public async Task<IActionResult> RecommandCoaches(int id)
        {
            var result = await _sCoachMethods.CoachRecommand(id);
            if (result == null || result.Count == 0)
                return Ok("教練們休息中");
            return Ok(new { isSuccess = true, data = result });

        }
        #endregion


        #endregion

        #region 課程
        #region 課程模板建立
        [Authorize]
        [HttpPost("AddCourseT")]
        public async Task<IActionResult> addCourseT([FromForm] DCourseCreate dto)
        {

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var result = await _sCoachMethods.CreateTemplate(dto, currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest(new { message = "申請失敗，請檢查資料是否正確" });
        }

        #endregion

        #region 編輯模板
        [Authorize]
        [HttpPut("EditCourseTemplate/{tempId}")]
        public async Task<IActionResult> EditCourseTemplate([FromForm] DCourseTempEdit dto, int tempId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var result = await _sCoachMethods.EditTemplate(dto, tempId, currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest(new { message = "申請失敗，請檢查資料是否正確" });
        }
        #endregion

        #region 模板展示
        [Authorize]
        [HttpGet("Temp")]
        public async Task<IActionResult> ThisTemplate()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var result = await _sCoachMethods.ThisTemp(userId);
            return Ok(new { isSuccess = true, data = result });
        }
        #endregion

        #region 課程選時間上架 
        [Authorize]
        [HttpPost("CourseTime/{templateId}")]
        public async Task<IActionResult> OpenTimeCourse([FromForm] DCourseOpenSession dto, int templateId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var result = await _sCoachMethods.OpenSession(dto, templateId, currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest(new { message = "申請失敗，請檢查資料是否正確" });
        }

        #endregion

        #region 課程時段刪除

        [Authorize]
        [HttpDelete("DeleteSession/{sessionId}")]
        public async Task<IActionResult> deleteThisSession(int sessionId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var result = await _sCoachMethods.DeleteCourseSession(sessionId, currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest(new { message = "刪除失敗" });
        }
        #endregion

        #region 日期找課
        [Authorize]
        [HttpGet("CourseDate/{coachId}/{day}")]
        public async Task<IActionResult> getCourseByDate(int coachId, string day)
        {
            try
            {
                var result = await _sCoachMethods.CourseByDates(day, coachId);
                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region 上架中
        [Authorize]
        [HttpGet("AllSessions")]
        public async Task<IActionResult> GetAllSessions()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();
            try
            {
                var result = await _sCoachMethods.GetAllPublishedSessions(userId);
                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }

        }
        #endregion

        #region 課程大眾展示介紹
        [HttpGet("CourseInfo/{sessionId}")]
        public async Task<IActionResult> CourseInfo(int sessionId)
        {
            if (sessionId == 0) return NotFound("新課程還在趕工中");
            try
            {
                var result = await _sCoachMethods.ThisCourse(sessionId);
                return Ok(new { isSuccess = true, data = result });

            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }

        }

        #endregion

        #region 教練最新課程展示
        [HttpGet("LatestCourse/{coachId}")]
        public async Task<IActionResult> LatestCourse(int coachId)
        {
            if (coachId == 0) return NotFound("找不到教練");
            try
            {
                var result = await _sCoachMethods.LatestCourseByCoach(coachId);

                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
        #endregion

        #region 參加過的課
        [Authorize]
        [HttpGet("AttendHistory")]
        public async Task<IActionResult> MySessionHistory()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            try
            {
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    var result = await _sCoachMethods.GetUserCourseHistory(currentUserId);
                    return Ok(new { isSuccess = true, data = result });
                }
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest(new { message = "讀取失敗，請檢查資料是否正確" });
        }
        #endregion
        #endregion

        #region~~專業~~
        [HttpGet("Spe")]
        public async Task<IActionResult> AllSpeciality()
        {
            try
            {
                var result = await _sCoachMethods.Sports();
                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }

        }
        #endregion

        #region 收藏
        #region 收藏  

        [HttpPost("Favorites")]
        public async Task<IActionResult> MyFavCoach(DCoachFav dto, int userId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

            int currentUserId = 0;

            if (userIdClaim != null)
            {
                // 抓到了！轉成數字
                currentUserId = int.Parse(userIdClaim.Value);
            }
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"類型: {c.Type}, 內容: {c.Value}");
            }

            // 2. 丟給 Service (Service 裡面有 UserId == 0 的守衛條款)
            var result = await _sCoachIndex.MyFavCoach(dto, currentUserId);

            // 3. 根據結果回傳 (注意：這裡的 IsSuccess 要跟前端判斷的一致)
            if (result == "請先登入後才能收藏喔！")
            {
                return Ok(new { IsSuccess = false, data = result });
            }

            return Ok(new { IsSuccess = true, data = result });
        }
        #endregion

        #region 查看收藏(保持愛心) 
        //[Authorize]
        [HttpGet("FavHeart")]
        public async Task<IActionResult> GetHeart()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 沒登入 → 回傳空清單，不要回 401
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Ok(new { Issuccess = true, data = new List<int>() });
            }

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var favIds = await _sCoachMethods.HeartIds(currentUserId);
                return Ok(new { Issuccess = true, data = favIds });
            }

            return Ok(new { Issuccess = true, data = new List<int>() });
        }

        #endregion

        #region 收藏清單
        [Authorize]
        [HttpGet("myFavList")]
        public async Task<IActionResult> MyFavCoachList(int page = 1, int pageSize = 8)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入啦" });
            }
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.GetMyFavCoach(currentUserId, page, pageSize);
                if (result == null || !result.Any())
                    return Ok(new { Issuccess = true, data = new List<object>() }); // 回傳空陣列
                return Ok(new { Issuccess = true, data = result });
            }
            return BadRequest("登入後才能使用功能");
        }
        #endregion

        #endregion


        #region 評論
        //TODO尚未測試
        #region 新增評論
        [Authorize]
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] DReview dto, int courseOrderId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized(new DAPIResponse<string> { IsSuccess = false, Message = "無效的憑證，請重新登入" });
            }

            if (!ModelState.IsValid) return BadRequest(new DAPIResponse<string> { IsSuccess = false, Message = "資料格式錯誤" });
            try
            {
                var result = await _sCoachMethods.CreateReview(dto, currentUserId, courseOrderId);

                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest();
        }

        #endregion
        #region 編輯評論
        [Authorize]
        [HttpPut("EditReview/{reviewId}")]
        public async Task<IActionResult> EditThisRevwew(DReview dto, int courseId, int reviewId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized(new DAPIResponse<string> { IsSuccess = false, Message = "無效的憑證，請重新登入" });
            }
            if (!ModelState.IsValid) return BadRequest(new DAPIResponse<string> { IsSuccess = false, Message = "資料格式錯誤" });

            var result = await _sCoachMethods.EditReview(dto, currentUserId, courseId, reviewId);
            try
            {
                return Ok(new { isSuccess = true, data = result });
                            }
            catch (Exception ex)
            {
                return Ok(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            return BadRequest();
        }

        #endregion
        #region 刪除評論
        [Authorize]
        [HttpDelete("DeleteReview/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized(new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = "無效的憑證，請重新登入"
                });
            }
            try
            {
                var result = await _sCoachMethods.DeleteReview(currentUserId, reviewId);
                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }

        }


        #endregion
        #region 查看評論
        [HttpGet("ContentDetails/{coachId}")]
        public async Task<IActionResult> ContentDetails(int coachId)
        {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            try
            {
                var result = await _sCoachMethods.CoachReviews(coachId);
                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        #endregion
        #region 拿最新三筆
        [HttpGet("ThreeReviews/{coachId}")]
        public async Task<IActionResult> ThreeReviews(int coachId)
        {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            try
            {
                var result = await _sCoachMethods.ThreeReviewsByCoach(coachId);
                return Ok(new { isSuccess = true, data = result });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        #endregion
        #endregion

        #region 交易
        //TODO 測試
        #region 預約課程
        [Authorize]
        [HttpPost("Reserve")]
        public async Task<IActionResult> MyReserveSession(DCourseOrder dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                try
                {
                    var result = await _sCoachMethods.SessionReserve(dto, currentUserId);
                  return Ok(new { isSuccess = true, data = result });

                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            return BadRequest(new { message = "失敗失敗" });
        }
        #endregion


        #endregion

        #region 營運 
        #endregion

        #region 
        #endregion

    }
}


    