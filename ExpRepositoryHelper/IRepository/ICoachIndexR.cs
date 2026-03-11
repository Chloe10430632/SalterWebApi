using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalterEFModels;

namespace ExpRepositoryHelper.IRepository
{
    public interface ICoachIndexR
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
        Task<string> CoachSpeciallity(int coachID);
        /**收藏*/
        Task<int> FavoriteCoach();

    }
}
