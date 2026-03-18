using Microsoft.EntityFrameworkCore;
using SalterEFModels;
using ExpRepositoryHelper.IRepository;
using SalterEFModels.EFModels;

namespace ExpRepositoryHelper.Repository
{
    public class RCoachIndex : IRCoachIndex
    {
        private readonly SalterDbContext _dbContext;
        public RCoachIndex(SalterDbContext dbContext) { _dbContext = dbContext; }

        public async Task<bool> ExistAsync(int userID, int coachID)
        {
            //return await _dbContext.ExpFavorites.AnyAsync((System.Linq.Expressions.Expression<Func<ExpFavorites, bool>>)(f => f.UserId == userID && f.CoachId == coachID));
            //TODO 檢查結果是否正確
            return await _dbContext.ExpFavorites.AnyAsync(f => f.UserId == userID && f.CoachId == coachID);
        }
        //TODO 檢查結果是否正確2
        public async Task AddFavCoach(ExpFavorite favEntity)
        {
            await _dbContext.ExpFavorites.AddAsync(favEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteFavCoach(int userID, int coachID)
        {
            var target = await _dbContext.ExpFavorites
            .FirstOrDefaultAsync(f => f.UserId == userID && f.CoachId == coachID);
            if (target != null)
            {
                //TODO 檢查結果是否正確3
                _dbContext.ExpFavorites.Remove(target);
                await _dbContext.SaveChangesAsync();
            }
        }

    }

}

