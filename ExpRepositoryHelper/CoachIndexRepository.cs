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

        public Task<string> CoachSpeciality()
        {
            throw new NotImplementedException();
        }

        public Task<int> CommentCount()
        {
            throw new NotImplementedException();
        }

        public Task<int> CommentPoint()
        {
            throw new NotImplementedException();
        }

        public Task<int> FavoriteCoach()
        {
            throw new NotImplementedException();
        }

        
    }
}
