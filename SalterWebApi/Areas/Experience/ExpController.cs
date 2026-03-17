using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System.Security.Claims;
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
        private readonly SalterDbContext _context;
        public ExpController(ISCoachIndex sCoachIndex, ISCoachMethods sCoachMethods, SalterDbContext db)
        {
            _sCoachIndex = sCoachIndex;
            _sCoachMethods = sCoachMethods;
            _context = db;
        }
        #endregion
        #region ~~入口~~
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

        #region~~教練~~
        #region 查看評論
        [HttpGet("ContentDetails{id}")]
        public async Task<IActionResult> ContentDetails(int coachId) {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            var result = await _sCoachMethods.CoachReviews(coachId);
            return Ok(result);
        }
        #endregion
        #region 申請加入教練(新增)
        [Authorize]
            [HttpPost("BecomeCoach")]
            public async Task<IActionResult> BecomeCoach(DCoachEdit dto ) {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
                   ?? User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdStr))
            {
                // 為了 debug，我們把抓到的所有 Type 列出來看看
                var allTypes = string.Join(", ", User.Claims.Select(c => c.Type));
                return Unauthorized($"抓不到 ID 標籤。目前有的標籤是: {allTypes}");
            }

            // 2. 轉 int
            if (int.TryParse(userIdStr, out int currentUserId))
            {
                var result = await _sCoachMethods.CreateCoach(dto, currentUserId);
                return Ok(result);
            }

            return BadRequest("ID 格式不正確，無法申請");
        }
        #endregion

        #region 教練自介//??找不到??
        [HttpGet("Info{id}")]
        public async Task<IActionResult> CoachInfo(int coachId) {
            if (coachId == 0) return NotFound("這位教練還沒出生");
            var result = await _sCoachMethods.ThisCoachInfo(coachId);
            return Ok(result);
        }
        #endregion

        #region 編輯自介
        [Authorize]
        [HttpPut("EditCoach{id}")]
        public async Task<IActionResult> EditThisCoach([FromBody] DCoachEdit dto) {
            // 1. 抓取 JWS 裡面的 UserId
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
                    ?? User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdStr))
            {
                // 為了 debug，我們把抓到的所有 Type 列出來看看
                var allTypes = string.Join(", ", User.Claims.Select(c => c.Type));
                return Unauthorized($"抓不到 ID 標籤。目前有的標籤是: {allTypes}");
            }

            // 2. 呼叫 Service，傳入 DTO 和 登入者 ID
            if (int.TryParse(userIdStr, out int currentUserId)) { 
                var result = await _sCoachMethods.EditCoachInfo(dto, currentUserId);
                            if (result.IsSuccess) return Ok(result);
            }                
            return BadRequest("系統忙碌中");
            
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
            public async Task<IActionResult> MyFavCoach(DCoachFav dto)
            {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
               ?? User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            if (string.IsNullOrEmpty(userIdStr))
            {
                // 為了 debug，我們把抓到的所有 Type 列出來看看
                var allTypes = string.Join(", ", User.Claims.Select(c => c.Type));
                return Unauthorized($"抓不到 ID 標籤。目前有的標籤是: {allTypes}");
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
        [HttpGet("test-mapping")]
        public async Task<IActionResult> TestMapping()
        {
            var coachData = await _context.ExpCoaches
    .Include(c => c.Specialities)  // 這裡就是你程式碼裡的 d.Specialities
    .Include(c => c.TripDistricts) // 這裡就是你程式碼裡的 d.TripDistricts
    .Select(c => new
    {
        CoachName = c.Name,
        // 把專長名稱抓成清單
        Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
        // 把地區名稱抓成清單
        Districts = c.TripDistricts.Select(d => d.Name).ToList()
    })
    .ToListAsync();
            return Ok(coachData);
            //var districtData = await _context.TripDistricts
            //.Include(d => d.CoachDists) // 對應你程式碼裡的 p.CoachDists
            //.ToListAsync();
            //return Ok(districtData);

        }




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
