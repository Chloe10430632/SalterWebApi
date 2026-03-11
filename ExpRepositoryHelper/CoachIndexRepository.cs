using Microsoft.EntityFrameworkCore;
using SalterEFModels;
using ExpRepositoryHelper.IRepository;
using SalterEFModels.EFModels;

namespace ExpRepositoryHelper
{
    public class CoachIndexRepository : ICoachIndexR
    {
        private readonly SalterEFModels.EFModels.SalterDbContext _dbContext;
        public CoachIndexRepository(SalterDbContext dbContext) { _dbContext = dbContext; }
        
        public async Task<DateTime?> CoachCreateTime(int coachID)
        {
            return await _dbContext.ExpCoaches
                .Where(c => c.Id == coachID)
                .Select(c => c.CreatedAt)
                .FirstOrDefaultAsync();
                
                }

        public async Task<string> CoachDistrict(int coachID)
        {
            var districts = await _dbContext.ExpCoaches
                            .Where(c => c.Id == coachID)
                            .SelectMany(c => c.TripDistricts) // 展開集合
                            .Select(d => d.Name)      // 只取名稱
                            .ToListAsync();                   // 先轉成 List

            // 將 List<string> 合併為單一字串，以逗號隔開
            return string.Join(", ", districts);
        }

        public async Task<string?> CoachSpeciallity(int coachID)
        {
            var speciallity = await _dbContext.ExpCoaches
                             .Where(c => c.Id == coachID)
                             .SelectMany(c => c.Specialities)
                             .Select(s => s.SportsName)
                             .ToListAsync();
            return string.Join("," , speciallity);


        }

        public async Task<int> CommentCount(int coachID)
        {
            var ccount = await _dbContext.ExpCoaches
                         .Where(c => c.Id == coachID)
                         .Select(c => c.ExpReviews)
                         .CountAsync();
            return ccount;
        }

        public async Task<List<int>> CommentScore(int coachID)
        {
            return await _dbContext.ExpReviews
                         .Where(r =>r.CoachId == coachID)
                         .Select(r => r.Rating ?? 0) //如果回傳是null 就變成0
                         .ToListAsync();
        }

        public Task<int> FavoriteCoach()
        {
            throw new NotImplementedException();
        }

        
    }
}
