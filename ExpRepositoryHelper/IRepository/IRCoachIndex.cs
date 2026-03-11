using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalterEFModels;
using SalterEFModels.EFModels;

namespace ExpRepositoryHelper.IRepository
{
    public interface IRCoachIndex
    {
        /**評分數*/
        Task<List<int>> CommentScore(int coachID);
        /**評論則數*/
        Task<int> CommentCount(int coachID);
        /**教練建立時間*/
        Task<DateTime?> CoachCreateTime(int coachID);
        /**地區*/
        Task<string> CoachDistrict(int coachID);
        /**專業*/
        Task<string?> CoachSpeciallity(int coachID);
        /**收藏數*/
        Task<int> FavoriteCoachCount(int coachID);
        /**加入收藏*/
        Task AddFavCoach(ExpFavorite favEntity); //存進去就好所以不用型別
        /**移除收藏*/
        Task DeleteFavCoach(int userID, int coachID);
        /**檢查存在*/
        Task<bool> ExistAsync(int userID, int coachID);


    }
}
