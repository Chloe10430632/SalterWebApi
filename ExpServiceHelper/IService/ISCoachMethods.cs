using Azure;
using CloudinaryDotNet.Actions;
using ExpServiceHelper.DTO;
using ExpServiceHelper.Service;
using Microsoft.AspNetCore.Http;
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
        /**搜尋-名字*/
        Task<List<DCoachInfo>> GetCoachName(string keyName);
        /**搜尋-地區*/
        Task<List<DCoachInfo>> GetCoachDist(string keyDistrict);

        /**搜尋-專業*/
        Task<List<DCoachInfo>> GetCoachSpecial(string keySpecial);
        /**排序-最新*/
        Task<List<DCoachInfo>> GetCoachNewest();
        /**排序-熱門*/
        Task<List<DCoachInfo>> CoachPopular(int page,int pageSize);
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

        #region ~~課程~~
        #region 課程模板建立
        Task<DAPIResponse<DCourseCreate>> CreateTemplate(DCourseCreate dto, int coachId);
        #endregion
        #region 課程模板編輯
        Task<DAPIResponse<DCourseTempEdit>> EditTemplate(DCourseTempEdit dto, int TemplateId, int currentUserId);
        #endregion
        #region  課程上架 
        Task<DAPIResponse<DCourseOpenSession>> OpenSession(DCourseOpenSession dto, int TemplateId, int currentUserId);
        #endregion
        #region 課程時段刪除
        Task<DAPIResponse<string>> DeleteCourseSession(int courseSessionId, int currentUserId);
        #endregion
        #region 課程展示
        Task<DAPIResponse<DCourseInfo>> ThisCourse(int courseId, int coachId);
        #endregion
        #endregion

        #region 評論
        #region 新增評論
        Task<DAPIResponse<string>> CreateReview(DReview dto, int userId, int courseOId);
        #endregion
        #region 編輯評論
        Task<DAPIResponse<DReview>> EditReview(DReview dto, int userId, int courseId, int reviewId);
        #endregion
        #region 刪除評論
        Task<DAPIResponse<string>> DeleteReview(int userId, int reviewId);
        #endregion
        #endregion


        #region 交易
        #region 預約課程
        Task<DAPIResponse<string>> CourseReserve(DCourseOrder dto, int userId, int courseSessionId);
        #endregion
        #region 支付 
        #endregion
        #region 歷史交易紀錄 
        #endregion
        #endregion

        #region 營運 
        #endregion

    }
}
