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
    public class SCoachMethods : ISCoachMethods
    {
        #region 
        #endregion
        #region DI
        private readonly SalterDbContext _context;
        public SCoachMethods(SalterDbContext dbContext) { _context = dbContext; }
        #endregion

        #region 入口

        /**搜尋-地區*/ //找不到//
        public async Task<List<DCoachInfo>> GetCoachDist(string keyDistrict)
        {
            var q = _context.ExpCoaches
                .Where(c => c.TripDistricts.Any(w => w.Name.Contains(keyDistrict)))
                .Select(c => new DCoachInfo
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = string.Join(",", c.TripDistricts.Select(m => m.Name)),
                    ReviewCount = c.ExpReviews.Count(),
                    AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    Specialities = c.Specialities.Select(s => s.SportsName).ToList()
                }
                );
            return await q.ToListAsync();

        }

        /**搜尋-專業*/ //找不到//
        public async Task<List<DCoachInfo>> GetCoachSpecial(string keySpecial)
        {

            var q = _context.ExpCoaches
                .Where(c => c.TripDistricts.Any(w => w.Name.Contains(keySpecial)))
                .Select(c => new DCoachInfo
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = string.Join(",", c.TripDistricts.Select(m => m.Name)),
                    ReviewCount = c.ExpReviews.Count(),
                    AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    Specialities = c.Specialities.Select(s => s.SportsName).ToList()
                }
                );
            return await q.ToListAsync();
        }

        /**排序-最新*/
        public async Task<List<DCoachInfo>> GetCoachNewest()
        {
            var q = _context.ExpCoaches
                .OrderByDescending(c => c.CreatedAt)
                .Take(12)
                .Select(c => new DCoachInfo
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = string.Join(",", c.TripDistricts.Select(m => m.Name)),
                    ReviewCount = c.ExpReviews.Count(),
                    AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
                    CreatedAt = c.CreatedAt
                });
            return await q.ToListAsync();
        }



        /**排序-熱門*/
        public async Task<List<DCoachInfo>> CoachRecommand()
        {
            //拿資料 算平均 算評論數
            var q = _context.ExpCoaches
                .Select(c => new
                {
                    Coach = c,
                    AvgRating = c.ExpReviews.Any() ? c.ExpReviews.Average(r => (double)r.Rating) : 0,
                    ReviewCount = c.ExpReviews.Count()
                });
            //排序
            var rank = await q.OrderByDescending(r => r.AvgRating)
                .ThenByDescending(r => r.ReviewCount)
                .Take(12)
                .ToListAsync();
            //包成DTO ->return
            return rank.Select(x => new DCoachInfo
            {
                CoachId = x.Coach.Id,
                CoachName = x.Coach.Name,
                AvatarUrl = x.Coach.AvatarUrl,
                District = string.Join(",", x.Coach.TripDistricts.Select(d => d.Name)),
                ReviewCount = x.ReviewCount,
                AvgRating = Math.Round(x.AvgRating, 1),
                Specialities = x.Coach.Specialities.Select(s => s.SportsName).ToList()
            }).ToList();


        }

        #endregion


        #region 教練編輯 ??mapAPI抓詳細地址??上傳圖片??
       public async Task<DAPIResponse<DEditCoach>> EditCoachInfo(DEditCoach dto, int currentUserId)
        {
            // 除了找 Coach ID，還要確認 user_id 也是本人
            var thisCoach = await _context.ExpCoaches
                .FirstOrDefaultAsync(c => c.Id == dto.Id && c.UserId == currentUserId);

            if (thisCoach == null){
                return new DAPIResponse<DEditCoach> { IsSuccess = false, Message = "權限不足或找不到教練資料" };
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
                .Select(c => new DEditCoach
                {
                    Id = c.Id,
                    Name = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    Introduction = c.Introduction,
                    CityId = c.CityId,
                    CityName = c.City.Name,
                    DistrictId = c.DistrictId,
                    DistrictName = c.District.Name
                }).FirstOrDefaultAsync();

            return new DAPIResponse<DEditCoach> { IsSuccess = true, Message = "更新成功！教練大人進化了！", Data = resultData };

        }
        #endregion

        #region 系統推薦教練
        public async Task<List<DCoachRecommend>> CoachRecommand(int thisCoachId) {
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
                .Where(c => c.Id != thisCoachId ) // 排除自己
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

            return recommendedList;
        }
        #endregion

    }
}
