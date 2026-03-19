using ExpServiceHelper.DTO;
using ExpServiceHelper.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        #region ~~教練~~
        #region 申請成為教練
        Task<DAPIResponse<int>> CreateCoach(DCoachEdit dto, int currentUserId);
        #endregion
        #region 教練編輯
        Task<DAPIResponse<DCoachEdit>> EditCoachInfo(DCoachEdit dto, int currentUserId);
        #endregion
        #region 詳細自介
        Task<DCoachInfo> ThisCoachInfo(int coachId);
        #endregion
        #region 系統推薦
        Task<List<DCoachRecommend>> CoachRecommand(int thisCoachId);
        #endregion
        #region 查看評論
        Task<List<DCoachReview>> CoachReviews(int coachId);
        #endregion
        # region 列出所有收藏
        Task<List<DCoachFavList>> GetMyFavCoach(int userId);
        #endregion
        #endregion

        #region 課程
        #region 課程模板建立
        Task<DAPIResponse<int>> CreateTemplate(DCourseCreate dto, int coachId);
        #endregion
        #region 課程模板編輯
        Task<DCourseEdit> EditTemplate(DCourseEdit dto, int TemplateId);
        #endregion
        #region 課程介紹get{id}
        #endregion
        #region 課程刪除
        #endregion
        #region 預約課程
        #endregion
        #region 新增評論
        #endregion
        #region 編輯評論
        #endregion
        #region 刪除評論
        #endregion
        #endregion

        #region 交易
        #region 支付 
        #endregion
        #region 歷史交易紀錄 
        #endregion
        #endregion

        #region 營運 
        #endregion

    }
}
