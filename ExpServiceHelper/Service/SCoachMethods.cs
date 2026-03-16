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

        /**搜尋-地區*/
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

        /**搜尋-專業*/
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


        #region 教練編輯 ??mapAPI抓詳細地址??
        public async Task<DAPIResponse<DEditCoachInfo>> EditCoachInfo(DEditCoachInfo dto)
        {
            var coach = await _context.ExpCoaches.FindAsync(dto.Id);
            if (coach == null)
            {
                return new DAPIResponse<DEditCoachInfo> { IsSuccess = false, Message = "謎樣人物" };
            }

            coach.Name = dto.Name;
            coach.AvatarUrl = dto.AvatarUrl;
            coach.Introduction = dto.Introduction;
            coach.DistrictId = dto.DistrictId;
            coach.CityId = dto.CityId;

            coach.UpdatedAt = DateTime.Now;
            if (coach.Name == null) coach.CreatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new DAPIResponse<DEditCoachInfo>() { IsSuccess = true, Message = "更新成功！教練大人進化了！", Data = dto };

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
