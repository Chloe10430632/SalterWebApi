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
        /**加入收藏*/
        Task AddFavCoach(ExpFavorites favEntity); //存進去就好所以不用型別
        /**移除收藏*/
        Task DeleteFavCoach(int userID, int coachID);
        /**檢查存在*/
        Task<bool> ExistAsync(int userID, int coachID);


    }
}
