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
        public ExpController(ISCoachIndex sCoachIndex, ISCoachMethods sCoachMethods) 
        {
            _sCoachIndex = sCoachIndex;
            _sCoachMethods = sCoachMethods;
        }
        #endregion
        #region ~~入口~~
        #region 排序
        #region 熱門排序 
        [HttpGet("PopRank")]
        public async Task<IActionResult> PopRank(int page = 1, int pageSize = 6)
        {
            var result = await _sCoachMethods.CoachPopular(page, pageSize);
            return Ok(new { Issuccess = true, data = result });
        }
        #endregion
        #region 最新排序 
        [HttpGet("NewRank")]
        public async Task<IActionResult> NewRank(int page = 1, int pageSize = 6)
        {
            var result = await _sCoachMethods.GetCoachNewest(page, pageSize);
            return Ok(new { Issuccess = true, data = result });
        }
        #endregion

        #endregion
        #region 搜尋

        #region~~搜尋-名字~~
        [HttpGet("NameSearch")]
        public async Task<IActionResult> NameSearch([FromQuery] string key)
        {
            try {
                var result = await _sCoachMethods.GetCoachName(key);
                return Ok(new { Issuccess = true, data = result });
            }
            catch (Exception ex) {
                BadRequest(new { message = ex.Message });
            }
            return BadRequest(new { message = "請檢查資料是否正確" });
        }
        #endregion

        #region~~搜尋-專業~~
        [HttpGet("SpeSearch")]
        public async Task<IActionResult> SpeSearch([FromQuery] string key)
        {
            try {
                var result = await _sCoachMethods.GetCoachSpecial(key);
                return Ok(new { Issuccess = true, data = result });
            }
            catch (Exception ex) { BadRequest(new { message = ex.Message }); }
            return BadRequest(new { message = "請檢查資料是否正確" });
        }
        #endregion

        #region~~搜尋-地區~~
        [HttpGet("DistSearch")]
        public async Task<IActionResult> DistSearch([FromQuery] string key)
        {
            try {
                var result = await _sCoachMethods.GetCoachDist(key);
                return Ok(new { Issuccess = true, data = result });
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

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.CreateCoach(dto, currentUserId);

                if (result.IsSuccess) Ok(new { Issuccess = true, data = result, message = "申請成功！歡迎加入教練行列" });
                return BadRequest(new { message = "你已經是教練了" });
            }
            return BadRequest(new { message = "ID 格式不正確" });
        }
        #endregion

        #region 編輯自介 
        [Authorize]
        [HttpPut("EditCoach/{coachId}")]
        public async Task<IActionResult> EditThisCoach([FromForm] DCoachEdit dto, int coachId)
        {
            // 1. 抓取 JWS 裡面的 UserId
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }

            // 2. 呼叫 Service，傳入 DTO 和 登入者 ID
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.EditCoachInfo(dto, currentUserId);
                if (result.IsSuccess) return Ok(new { Issuccess = true, data = result });
            }
            return BadRequest("系統忙碌中");

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
                return NotFound("教練們休息中");
            return Ok(new { Issuccess = true, data = result });
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

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.CreateTemplate(dto, currentUserId);
                if (result.IsSuccess) return Ok(new { message = "課程模板建好啦！" });
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

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                try {
                    var result = await _sCoachMethods.EditTemplate(dto, tempId, currentUserId);
                    if (result.IsSuccess) return Ok(result);
                }
                catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
            }
            return BadRequest(new { message = "申請失敗，請檢查資料是否正確" });
        }
        #endregion

        #region 課程選時間上架 
        [Authorize]
        [HttpPost("CourseTime/{templateId}")]
        public async Task<IActionResult> OpenTimeCourse([FromBody] DCourseOpenSession dto, int templateId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                try
                {
                    var result = await _sCoachMethods.OpenSession(dto, templateId, currentUserId);
                    if (result.IsSuccess) return Ok(result);
                }
                catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
            }
            return BadRequest(new { message = "申請失敗，請檢查資料是否正確" });
        }

        #endregion

        #region 課程時段刪除

        [Authorize]
        [HttpDelete("DeleteSession/{sessionId}")]
        public async Task<IActionResult> deleteThisSession(int courseSessionId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                try
                {
                    var result = await _sCoachMethods.DeleteCourseSession(courseSessionId, currentUserId);
                    if (result.IsSuccess) return Ok(result);
                }
                catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
            }
            return BadRequest(new { message = "刪除失敗" });
        }
        #endregion

        #region 課程展示介紹
        [HttpGet("CourseInfo/{sessionId}")]
        public async Task<IActionResult> CourseInfo(int sessionId)
        {
            if (sessionId == 0 ) return NotFound("新課程還在趕工中");
            try
            {
                var result = await _sCoachMethods.ThisCourse(sessionId);
                if (!result.IsSuccess) return NotFound(result.Message);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }

        }
        [HttpGet("LatestCourse/{coachId}")]
        public async Task<IActionResult> LatestCourse(int coachId)
        {
            if (coachId == 0) return NotFound("找不到教練");
            try
            {
                var result = await _sCoachMethods.LatestCourseByCoach(coachId);
                
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
        #endregion
        #endregion

        #region 收藏
        #region 收藏  
        [Authorize]
        [HttpPost("Favorites")]
        public async Task<IActionResult> MyFavCoach(DCoachFav dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }
            //轉int
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                //多傳入一個 currentUserId
                var result = await _sCoachIndex.MyFavCoach(dto, currentUserId);
                return Ok(new { Issuccess = true, data = result });
            }
            return BadRequest("登入後才能使用功能");
        }
        #endregion

        #region 查看收藏(保持愛心) 
        [Authorize]
        [HttpGet("FavHeart")]
        public async Task<IActionResult> GetHeart()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入" });
            }
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var favIds = await _sCoachMethods.HeartIds(currentUserId);
                return Ok(new { Issuccess = true, data = favIds });
            }
            return BadRequest("沒有收藏");
        }

        #endregion

        #region 收藏清單
        [HttpGet("myFavList")]
        public async Task<IActionResult> MyFavCoachList(int userId, int page = 1, int pageSize = 6)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized(new { message = "無效的憑證，請重新登入啦" });
            }
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.GetMyFavCoach(userId, page, pageSize);
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
        public async Task<IActionResult> AddReview([FromBody] DReview dto, int courseId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized(new DAPIResponse<string> { IsSuccess = false, Message = "無效的憑證，請重新登入" });
            }

            if (!ModelState.IsValid) return BadRequest(new DAPIResponse<string> { IsSuccess = false, Message = "資料格式錯誤" });

            var result = await _sCoachMethods.CreateReview(dto, currentUserId, courseId);
            if (result.IsSuccess)
                return Ok(new { Issuccess = true, data = result });
            return BadRequest(new DAPIResponse<string>
            {
                IsSuccess = false,
                Message = "新增失敗"
            });
        }

        #endregion
        #region 編輯評論
        [Authorize]
        [HttpPut("EditReview/{reviewId}")]
        public async Task<IActionResult> EditThisRevwew(DReview dto, int courseId, int reviewId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId)) {
                return Unauthorized(new DAPIResponse<string> { IsSuccess = false, Message = "無效的憑證，請重新登入" });
            }
            if (!ModelState.IsValid) return BadRequest(new DAPIResponse<string> { IsSuccess = false, Message = "資料格式錯誤" });

            var result = await _sCoachMethods.EditReview(dto, currentUserId, courseId, reviewId);
            if (result.IsSuccess)
                return Ok(new { Issuccess = true, data = result });
            return BadRequest(new DAPIResponse<string>
            {
                IsSuccess = false,
                Message = "編輯失敗"
            });
        }

        #endregion
        #region 刪除評論
        [Authorize]
        [HttpDelete("DeleteReview/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId) {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int currentUserId)){
                    return Unauthorized(new DAPIResponse<string> {
                        IsSuccess = false, Message = "無效的憑證，請重新登入" });
            }
                try {
                    var result = await _sCoachMethods.DeleteReview(currentUserId,reviewId);
                    if (result.IsSuccess) return Ok(new { Issuccess = true, data = result });
                return BadRequest(new { message = result.Message });
                }
                catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
            
        }


        #endregion
        #region 查看評論
        [HttpGet("ContentDetails/{coachId}")]
        public async Task<IActionResult> ContentDetails(int coachId)
        {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            var result = await _sCoachMethods.CoachReviews(coachId);
            return Ok(new { Issuccess = true, data = result });
        }
        #endregion

        #endregion

        #region 交易
        //TODO 測試
        #region 預約課程
        [Authorize]
        [HttpPost("Reserve")]
        public async Task<IActionResult> MyReserveSession(DCourseOrder dto) {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new { message = "無效的憑證，請重新登入" });

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                try {
                    var result = await _sCoachMethods.SessionReserve(dto, currentUserId);
                    if (result.IsSuccess) return Ok(new { Issuccess = true, data = result });
                }
                catch (Exception ex) { return BadRequest(ex.Message); }
            }
            return BadRequest(new { message = "失敗失敗" });
            }
        #endregion
        #region 歷史交易紀錄 
        #endregion
        #endregion

        #region 營運 
        #endregion



    }
}
