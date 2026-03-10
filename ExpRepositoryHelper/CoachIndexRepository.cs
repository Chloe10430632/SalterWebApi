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
        public async Task<DateTime> CoachCreateTime()
        {
           var ct = _dbContext.ExpCoaches.
        }

        public Task<string> CoachDistrict()
        {
            throw new NotImplementedException();
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
