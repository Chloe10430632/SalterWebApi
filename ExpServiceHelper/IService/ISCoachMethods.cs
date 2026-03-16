using ExpServiceHelper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpServiceHelper.IService
{
    public interface ISCoachMethods
    {
        #region 入口
        /**搜尋-地區*/
        Task<List<DCoachInfo>> GetCoachDist(string keyDistrict);

        /**搜尋-專業*/
        Task<List<DCoachInfo>> GetCoachSpecial(string keySpecial);
        /**排序-最新*/
        Task<List<DCoachInfo>> GetCoachNewest();
        /**排序-熱門*/
        Task<List<DCoachInfo>> CoachRecommand();
        #endregion
        #region 教練編輯
        Task<DAPIResponse<DEditCoach>> EditCoachInfo(DEditCoach dto, int currentUserId);
        #endregion
        #region 系統推薦
        Task<List<DCoachRecommend>> CoachRecommand(int thisCoachId);
        #endregion

    }
}
