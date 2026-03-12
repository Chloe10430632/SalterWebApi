using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using SalterEFModels.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.Service
{
    public class SCoachMethods :ISCoachMethods
    {
        private readonly SalterDbContext _context;
        public SCoachMethods(SalterDbContext dbContext) { _context = dbContext; }

        //public Task<DCoachInfo> GetCoachDist(string keyDistrict)
        //{
        //    throw new NotImplementedException();
        //}

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

        /**搜尋-地區*/

        /**搜尋-專業*/

        /**排序-最新*/

        /**排序-熱門*/
    }
}
