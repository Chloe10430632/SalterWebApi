using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalterEFModels.EFModels;
using System.Security.Claims;
using static ExpServiceHelper.DTO.DFavCoach;


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
        #region 入口
            #region 排序
            [HttpGet("PopRank")]
            public async Task<IActionResult> PopRank()
            {
                var result = await _sCoachMethods.CoachRecommand();
                return Ok(result);
            }

            [HttpGet("NewRank")]
            public async Task<IActionResult> NewRank()
            {
                var result = await _sCoachMethods.GetCoachNewest();
                return Ok(result);
            }
            #endregion
            #region 搜尋
            [HttpGet("SpeSearch")]
            public async Task<IActionResult> SpeSearch(string keySpecial)
            {
                var result = await _sCoachMethods.GetCoachSpecial(keySpecial);
                if (result == null || result.Count == 0)
                    return NotFound("！太難了 教練不會！");
                return Ok(result);
            }
            [HttpGet("DistSearch")]
            public async Task<IActionResult> DistSearch(string keyDistrict)
            {
                var result = await _sCoachMethods.GetCoachDist(keyDistrict);
                //check
                if (keyDistrict == null || result.Count == 0)
                    return NotFound("！這裡沒有所謂教練這種生物！");
                return Ok(result);
            }
        #endregion
        #endregion

        #region 教練====
        #region 申請加入教練(新增)

        #endregion

        #region 詳細自介
        [HttpGet("Info{id}")]
        public async Task<IActionResult> CoachInfo(int coachId) {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            var result = await _sCoachMethods.ThisCoachInfo(coachId);
            return Ok(result);
        }
        #endregion

        #region 編輯自介put{id}
        [Authorize]
        [HttpPut("EditCoach{id}")]
        public async Task<IActionResult> EditThisCoach([FromBody] DEditCoach dto) {
            // 1. 抓取 JWS 裡面的 UserId
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized("請重新登入");
            }
            // 2. 呼叫 Service，傳入 DTO 和 登入者 ID
            var result = await _sCoachMethods.EditCoachInfo(dto, currentUserId);

            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        #endregion

        #region 系統推薦
        [HttpGet("Recommand{id}")]
            public async Task<IActionResult> RecommandCoaches(int id)
            {
                var result = await _sCoachMethods.CoachRecommand();
                if (result == null || result.Count == 0)
                    return NotFound("教練們休息中");
                return Ok(result);
            }
            #endregion

        #region 收藏
            [Authorize]
            [HttpPost("Favorites")]
            public async Task<IActionResult> MyFavCoach(DFavCoach dto)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdStr))
                {
                    return Unauthorized("找不到會員資訊");
                }
                //轉int
                if (int.TryParse(userIdStr, out int currentUserId))
                {
                    //多傳入一個 currentUserId
                    var result = await _sCoachIndex.MyFavCoach(dto, currentUserId);
                    return Ok(result);
                }
                return BadRequest("登入後才能收藏");
            }
        #endregion
        #endregion

        #region 課程
        #region 課程介紹get{id}
        #endregion
        #region 課程編輯post{id}
        #endregion
        #region 課程刪除
        #endregion
        #region 預約課程
        #endregion
        #region 新增評論
        #endregion
        #region 編輯評論
        #endregion
        #region 刪除評論
        #endregion
        #endregion

        #region 交易
        #region 支付 
        #endregion
        #region 歷史交易紀錄 
        #endregion
        #endregion

        #region 營運 
        #endregion





        // GET: api/<ExpController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ExpController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ExpController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ExpController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ExpController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
