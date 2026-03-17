using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExpServiceHelper.Service
{
# region 擴充方法區
    // --- 這裡是擴充方法區（放在最外層，不要住在別的 Class 裡面） ---public static class CoachMappingExtensions
    public static class CoachMappingExtensions
    {
        // 這是一個「擺盤範本」，不管是搜地區、搜名字、搜專長，都用這個範本
        public static IQueryable<DCoachInfo> SelectCoachInfo(this IQueryable<ExpCoach> query)
        {
            return query.Select(c => new DCoachInfo
            {
                CoachId = c.Id,
                CoachName = c.Name,
                AvatarUrl = c.AvatarUrl,
                // 提醒：在資料庫層級 string.Join 可能會報錯，建議到記憶體再處理，或者直接選成 List
                District = c.TripDistricts.Select(m => m.Name).ToList(),
                Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
                ReviewCount = c.ExpReviews.Count(),
                AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                CreatedAt = c.CreatedAt
            });
        }
    }
    #endregion

    public class SCoachMethods : ISCoachMethods
    {
        #region 
        #endregion
        #region DI
        private readonly SalterDbContext _context;
        public SCoachMethods(SalterDbContext dbContext) { _context = dbContext; }
        #endregion
        #region~~教練~~
        #region 入口
       
        /**搜尋-地區*/ 
        public async Task<List<DCoachInfo>> GetCoachDist(string keyDistrict)
        {
            var result = await _context.ExpCoaches
                .Where(c => c.TripDistricts.Any(m => m.Name.Contains(keyDistrict)))
                .SelectCoachInfo() // 呼叫擴充方法
                .ToListAsync();

            // 如果你一定要回傳 string District，可以在這裡跑個 foreach 做 string.Join
            return result;
        }
       

        /**搜尋-專業*/
        public async Task<List<DCoachInfo>> GetCoachSpecial(string keySpecial)
        {
           var result = await _context.ExpCoaches
                .Where( c=> c.Specialities.Any(s => s.SportsName.Contains(keySpecial)))
                .SelectCoachInfo()
                .ToListAsync();
            return result;
        }

        /**排序-最新*/
        public async Task<List<DCoachInfo>> GetCoachNewest()
        {
            var result = await _context.ExpCoaches
                 .OrderByDescending(c => c.CreatedAt)
                 .Take(12)
                 .SelectCoachInfo()
                 .ToListAsync();
            return result;
        }



        /**排序-熱門*/
        public async Task<List<DCoachInfo>> CoachRecommand()
        {
            // 1. 只拿「排序需要」的資料
            var rankedCoachesQuery = _context.ExpCoaches
                .Select(c => new
                {
                    Entity = c, // 把整顆教練實體帶著
                    Avg = c.ExpReviews.Any() ? c.ExpReviews.Average(r => (double)r.Rating) : 0,
                    Count = c.ExpReviews.Count()
                })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .Take(12)
                .Select(x => x.Entity); // 【關鍵】排序完後，我們只要回傳教練實體

            // 2. 既然拿到了「前12名的教練實體」，直接接上你的擴充方法！
            return await rankedCoachesQuery.SelectCoachInfo().ToListAsync();
        }

        #endregion

        #region 申請加入教練(新增)
        public async Task<DAPIResponse<int>> CreateCoach(DCoachEdit dto, int currentUserId)
        {
            // 1. 檢查是否已經是教練（協作規範：一人只能有一個教練身份）
            bool exists = await _context.ExpCoaches.AnyAsync(c => c.UserId == currentUserId);
            if (exists) return new DAPIResponse<int> { IsSuccess = false, Message = "您已經是教練囉！" };

            // 2. 建立新實體
            var newCoach = new ExpCoach
            {
                UserId = currentUserId, // 綁定目前的 User
                Name = dto.Name,
                AvatarUrl = dto.AvatarUrl,
                Introduction = dto.Introduction,
                CityId = dto.CityId,
                DistrictId = dto.DistrictId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsSuspended = false // 預設不停權
            };

            _context.ExpCoaches.Add(newCoach);
            await _context.SaveChangesAsync();

            return new DAPIResponse<int>
            {
                IsSuccess = true,
                Message = "新教練誕生 ！",
                Data = newCoach.Id // 這就是你要的號碼牌！
            };
        }
        #endregion

        #region 詳細自介
        public async Task<DCoachInfo> ThisCoachInfo(int coachId)
        {
            var query = _context.ExpCoaches
                     .Where(c => c.Id == coachId)
                     .Select(c => new DCoachInfo
                     {
                         CoachId = c.Id,
                         CoachName = c.Name,
                         AvatarUrl = c.AvatarUrl,
                         // 注意：在 LINQ to Entities 中，直接 string.Join 可能會報錯，
                         // 建議先撈出資料到記憶體，或是處理方式調整
                        // District = c.District.Name, // 根據關聯圖，如果是 1對1 可以直接點出來
                         AvgRating = c.ExpReviews.Any() ? c.ExpReviews.Average(r => (double)r.Rating) : 0,
                         ReviewCount = c.ExpReviews.Count(),
                         Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
                         Introduction = c.Introduction
                     });

            // 關鍵在這裡！使用 FirstOrDefaultAsync() 真正去資料庫拿「第一筆或預設值」
            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        #endregion

        #region 教練編輯 ??mapAPI抓詳細地址??上傳圖片??
        public async Task<DAPIResponse<DCoachEdit>> EditCoachInfo(DCoachEdit dto, int currentUserId)
        {
            // 除了找 Coach ID，還要確認 user_id 也是本人
            var thisCoach = await _context.ExpCoaches
                .FirstOrDefaultAsync(c => c.UserId == currentUserId);

            if (thisCoach == null)
            {
                return new DAPIResponse<DCoachEdit> { IsSuccess = false, Message = "權限不足或找不到教練資料" };
            }

            // 2.(賦值)：把前端傳來的 dto 資料塞進資料庫的 entity 裡
            // 這一步才是真正的「更新」！
            thisCoach.Name = dto.Name;
            thisCoach.AvatarUrl = dto.AvatarUrl;
            thisCoach.Introduction = dto.Introduction;
            thisCoach.CityId = dto.CityId;
            thisCoach.DistrictId = dto.DistrictId;
            thisCoach.UpdatedAt = DateTime.Now;

            // 3. 存檔：這時候 EF 就會知道 thisCoach 被動過了，發出 UPDATE 指令
            await _context.SaveChangesAsync();

            // 4. 回傳：因為有只放id的欄位 所以要回傳抓新的對應名字
            var resultData = await _context.ExpCoaches
                .Where(c => c.Id == thisCoach.Id)
                .Select(c => new DCoachEdit
                {
                    Name = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    Introduction = c.Introduction,
                    CityId = c.CityId,
                    CityName = c.City.Name,
                    DistrictId = c.DistrictId,
                    DistrictName = c.District.Name
                }).FirstOrDefaultAsync();

            return new DAPIResponse<DCoachEdit> { IsSuccess = true, Message = "更新成功！教練大人進化了！", Data = resultData };

        }
        #endregion

        #region 系統推薦教練
        public async Task<List<DCoachRecommend>> CoachRecommand(int thisCoachId)
        {
            // 1. 先把「當前教練」的地區與專長抓出來（這是我們的基準點）
            var currentCoachInfo = await _context.ExpCoaches
                .Where(c => c.Id == thisCoachId)
                .Select(c => new
                {
                    CityId = c.CityId,
                    // 抓出他所有的專長 ID
                    SpecialityIds = c.Specialities.Select(s => s.SportsName).ToList()
                })
                .FirstOrDefaultAsync();

            if (currentCoachInfo == null) return new List<DCoachRecommend>();
            // 2. 開始找「臭味相投」的教練
            var query = _context.ExpCoaches
                .Where(c => c.Id != thisCoachId) // 排除自己
                .Where(c => c.CityId == currentCoachInfo.CityId &&
                            c.Specialities.Any(s => currentCoachInfo.SpecialityIds.Contains(s.SportsName)));
            // 3. 執行隨機排序並取前 3 名
            // 提示：在 LINQ to Entities 中，Guid.NewGuid() 常用來模擬隨機排序//
            var recommendedList = await query
                .OrderBy(c => Guid.NewGuid())
                .Take(3)
                .Select(c => new DCoachRecommend
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    // 這裡假設你有關聯到 TripDistricts
                    City = c.Name,
                    Specialities = c.Specialities
                                    .Select(s => s.SportsName)
                                    .ToList()
                })
                .ToListAsync();
            //+排除自己

            return recommendedList;
        }
        #endregion
        #endregion
        #region~~課程~~
        #region 課程介紹

        //public async Task<> CourseIntro(){}

        #endregion
        #endregion

        

    }
    #region 課程
    #region 課程介紹get{id}
    #endregion
    //CourseSaveasTemplate()
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
}
