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
        Task<int> CommentPoint();
        /**評論則數*/
        Task<int> CommentCount();
        /**教練建立時間*/
        Task<DateTime?> CoachCreateTime(int coachID);
        /**地區*/
        Task<string> CoachDistrict();
        /**專業*/
        Task<string> CoachSpeciality();
        /**收藏*/
        Task<int> FavoriteCoach();

    }
}
