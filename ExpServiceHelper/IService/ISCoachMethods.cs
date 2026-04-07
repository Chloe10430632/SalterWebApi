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
        Task<List<DCoachInfo>> GetCoachNewest(int page, int pageSize);
        /**排序-熱門*/
        Task<List<DCoachInfo>> CoachPopular(int page, int pageSize);
        #endregion

        #region ~~教練~~
        #region 申請成為教練
        Task<DCoachEdit> CreateCoach(DCoachEdit dto, int currentUserId);
        #endregion
        #region 教練編輯
        Task<DCoachEdit> EditCoachInfo(DCoachEdit dto, int currentUserId);
        #endregion
        #region 自己自介 
        Task<DCoachInfo> MyCoachInfo(int userId);
        #endregion
        #region 詳細自介
        Task<DCoachInfo> ThisCoachInfo(int coachId);
        #endregion
        #region 系統推薦
        Task<List<DCoachRecommend>> CoachRecommand(int thisCoachId);
        #endregion
        #endregion

        #region ~~課程~~
        #region 課程模板建立
        Task<DCourseCreate> CreateTemplate(DCourseCreate dto, int coachId);
        #endregion
        #region 課程模板編輯
        Task<DCourseTempEdit> EditTemplate(DCourseTempEdit dto, int TemplateId, int currentUserId);
        #endregion
        # region 模板展示
        Task<List<DCourseInfo>> ThisTemp(int currentUserId);
        #endregion
        #region  課程上架 
        Task<DCourseOpenSession> OpenSession(DCourseOpenSession dto, int TemplateId, int currentUserId);
        #endregion
        #region 日期找課 
        Task<List<DCourseInfo>> CourseByDates(string day, int coachId);
        #endregion
        #region 已上架
        Task<List<DCourseInfo>> GetAllPublishedSessions(int currentUserId);
        #endregion
        #region 課程時段刪除
        Task<string> DeleteCourseSession(int courseSessionId, int currentUserId);
        #endregion
        #region 課程展示
        Task<DCourseInfo> ThisCourse(int courseId);
        #endregion
        #region 參加過的課
        Task<List<DCourseOrder>> GetUserCourseHistory(int userId);
        #endregion
        #region 所有開課日-月曆
        Task<List<string>> GetCoachCourseDatesAsync(int coachId);
        #endregion
        #region 所有課
        Task<List<DCourseInfo>> GetCoursesByDateAsync(int coachId, DateOnly date);
        #endregion
        #region 最近開課
        Task<DCourseInfo> LatestCourseByCoach(int coachId);
        #endregion
        #endregion

        #region ~~收藏~~
        #region 列出所有收藏
        Task<List<DCoachFavList>> GetMyFavCoach(int userId, int page, int pageSize);
        #endregion
        #region 查看收藏(保持愛心) 
        Task<List<int>> HeartIds(int userId);
        #endregion

        #endregion

        #region 專業名稱 
        Task<List<DSpeciallity>> Sports();
        #endregion

        #region 評論
        #region 新增評論
        Task<string> CreateReview(DReview dto, int userId, int courseOId);
        #endregion
        #region 編輯評論
        Task<DReview> EditReview(DReview dto, int userId, int courseId, int reviewId);
        #endregion
        #region 刪除評論
        Task<string> DeleteReview(int userId, int reviewId);
        #endregion
        #region 查看評論
        Task<List<DCoachReview>> CoachReviews(int coachId);
        #endregion
        #region 最新三則
        Task<IEnumerable<DReview>> ThreeReviewsByCoach(int coachId);
        #endregion
        #endregion


        #region 交易
        #region 預約課程
        Task<object> SessionReserve(DCourseOrder dto, int userId);
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
