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
    public class SCoachMethods :ISCoachMethods
    {
        private readonly SalterDbContext _context;
        public SCoachMethods(SalterDbContext dbContext) { _context = dbContext; }


        /**搜尋-地區*/
        public async Task<List<DCoachInfo>> GetCoachDist(string keyDistrict)
        {
            var q =  _context.ExpCoaches
                .Where(c => c.TripDistricts.Any(w => w.Name.Contains(keyDistrict)))
                .Select(c => new DCoachInfo
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = string.Join(",", c.TripDistricts.Select(m => m.Name)),
                    ReviewCount = c.ExpReviews.Count(),
                     AverageRating = c.ExpReviews.Any()?Math.Round(c.ExpReviews.Average(r => (double)r.Rating),1):0,
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
                    AverageRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    Specialities = c.Specialities.Select(s => s.SportsName).ToList()
                }
                );
            return await q.ToListAsync();
        }

        /**排序-最新*/
       public async Task<List<DCoachInfo>>GetCoachNewest()
        {
            var q = _context.ExpCoaches
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new DCoachInfo
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = string.Join(",", c.TripDistricts.Select(m => m.Name)),
                    ReviewCount = c.ExpReviews.Count(),
                    AverageRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
                    CreatedAt = c.CreatedAt
                });
                  return await q.ToListAsync();
        }

        Task<List<DCoachInfo>> ISCoachMethods.GetCoachHottest()
        {
            throw new NotImplementedException();
        }

        /**排序-熱門*/
        //public async Task<List<DCoachInfo>> GetCoachHottest()
        // {
        //     //算平均


        //     return;
        // }

    }
}
