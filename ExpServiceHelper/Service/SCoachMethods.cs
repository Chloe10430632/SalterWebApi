using Azure;
using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExpServiceHelper.Service
{
    #region 擴充方法區
    // --- 這裡是擴充方法區（放在最外層，不要住在別的 Class 裡面） ---public static class CoachMappingExtensions
    public static class CoachMappingExtensions
    {
        // 這是一個「擺盤範本」，不管是搜地區、搜名字、搜專長，都用這個範本
        public static IQueryable<DCoachInfo> SelectCoachInfo(this IQueryable<ExpCoach> query)
        {
            return query.Select(c => new DCoachInfo
            {
                CoachId = c.Id,
                CoachName = c.Name,
                AvatarUrl = c.AvatarUrl,
                // 提醒：在資料庫層級 string.Join 可能會報錯，建議到記憶體再處理，或者直接選成 List
                District = c.TripDistricts.Select(m => m.Name).ToList(),
                Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
                ReviewCount = c.ExpReviews.Count(),
                AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                CreatedAt = c.CreatedAt
            });
        }

    }

    #endregion

    public class SCoachMethods : ISCoachMethods
    {
        #region 
        #endregion

        #region DI
        private readonly SalterDbContext _context;
        public SCoachMethods(SalterDbContext dbContext) { _context = dbContext; }
        #endregion

        #region 入口
        #region~~搜尋-名字~~
        public async Task<List<DCoachInfo>> GetCoachName(string key) { 
            var result = await _context.ExpCoaches
                .Where(c =>c.Name.Contains(key))
                .SelectCoachInfo()
                .ToListAsync();
            return result;
        }
        #endregion

        #region~~搜尋-地區~~
        public async Task<List<DCoachInfo>> GetCoachDist(string key)
        {
            var result = await _context.ExpCoaches
                .Where(c => c.TripDistricts.Any(m => m.Name.Contains(key)))
                .SelectCoachInfo() // 呼叫擴充方法
                .ToListAsync();

            // 如果你一定要回傳 string District，可以在這裡跑個 foreach 做 string.Join
            return result;
        }
        #endregion

        #region~~搜尋-專業~~
        public async Task<List<DCoachInfo>> GetCoachSpecial(string key)
        {
            var result = await _context.ExpCoaches
                 .Where(c => c.Specialities.Any(s => s.SportsName.Contains(key)))
                 .SelectCoachInfo()
                 .ToListAsync();
            return result;
        }
        #endregion

        #region~~排序-最新~~
        public async Task<List<DCoachInfo>> GetCoachNewest()
        {
            var result = await _context.ExpCoaches
                 .OrderByDescending(c => c.CreatedAt)
                 .Take(12)
                 .SelectCoachInfo()
                 .ToListAsync();
            return result;
        }
        #endregion

        #region~~排序-熱門~~
        public async Task<List<DCoachInfo>> CoachPopular(int page,int pageSize)
        {
            // 1. 只拿「排序需要」的資料
            var rankedCoachesQuery = _context.ExpCoaches
                .Select(c => new
                {
                    Entity = c, // 把整顆教練實體帶著
                    Avg = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    Count = c.ExpReviews.Count()
                })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .Select(x => x.Entity); // 【關鍵】排序完後，我們只要回傳教練實體

            // 2. 既然拿到了「前12名的教練實體」，直接接上你的擴充方法！
            return await rankedCoachesQuery.SelectCoachInfo().ToListAsync();
        }
        #endregion
        #endregion

        #region~~教練~~
        #region 查看評論
        public async Task<List<DCoachReview>> CoachReviews(int coachId)
        {
            var review = _context.ExpReviews
                .Where(r => r.CoachId == coachId && r.IsHidden != true)
                .OrderByDescending(r => r.ReviewedAt)
                .Select(r => new DCoachReview
                {
                    UserName = r.User.UserName,
                    CoachId = r.CoachId,
                    CourseOrderId = r.CourseOrderId,
                    Rating = r.Rating,
                    ReviewContent = r.ReviewContent,
                    ReviewedAt = r.ReviewedAt,
                });
            return await review.ToListAsync();
        }
        #endregion

        #region 申請加入教練(新增)  
        public async Task<DAPIResponse<int>> CreateCoach(DCoachEdit dto, int currentUserId)
        {
            // 1. 檢查是否已經是教練（協作規範：一人只能有一個教練身份）
            bool exists = await _context.ExpCoaches.AnyAsync(c => c.UserId == currentUserId);
            if (exists) return new DAPIResponse<int> { IsSuccess = false, Message = "您已經是教練囉！" };

            // 2. 建立新實體
            var newCoach = new ExpCoach
            {
                UserId = currentUserId, // 綁定目前的 User
                Name = dto.Name,
                AvatarUrl = dto.AvatarUrl,
                Introduction = dto.Introduction,
                //CityId = dto.CityId,
                DistrictId = dto.DistrictId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsSuspended = false // 預設不停權
            };

            _context.ExpCoaches.Add(newCoach);
            await _context.SaveChangesAsync();

            return new DAPIResponse<int>
            {
                IsSuccess = true,
                Message = "新教練誕生 ！",
                Data = newCoach.Id // 這就是你要的號碼牌！
            };
        }
        #endregion  

        #region 詳細自介
        public async Task<DCoachInfo> ThisCoachInfo(int coachId)
        {
            var query = _context.ExpCoaches
                     .Where(c => c.Id == coachId)
                     .Select(c => new DCoachInfo
                     {
                         CoachId = c.Id,
                         CoachName = c.Name,
                         AvatarUrl = c.AvatarUrl,
                         District = c.TripDistricts.Select(m => m.Name).ToList(),
                         AvgRating = c.ExpReviews.Any() ? Math.Round( c.ExpReviews.Average(r => (double)r.Rating) ,1): 0,
                         ReviewCount = c.ExpReviews.Count(),
                         Specialities = c.Specialities.Select(s => s.SportsName).ToList(),
                         Introduction = c.Introduction,
                         CreatedAt = c.CreatedAt

                     });

            // 關鍵在這裡！使用 FirstOrDefaultAsync() 真正去資料庫拿「第一筆或預設值」
            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        #endregion

        #region 教練編輯 ??mapAPI抓詳細地址??上傳圖片?? 
        public async Task<DAPIResponse<DCoachEdit>> EditCoachInfo(DCoachEdit dto, int currentUserId)
        {
            // 除了找 Coach ID，還要確認 user_id 也是本人
            var thisCoach = await _context.ExpCoaches
                .FirstOrDefaultAsync(c => c.UserId == currentUserId);

            if (thisCoach == null)
            {
                return new DAPIResponse<DCoachEdit> { IsSuccess = false, Message = "權限不足或找不到教練資料" };
            }

            // 2.(賦值)：把前端傳來的 dto 資料塞進資料庫的 entity 裡
            // 這一步才是真正的「更新」！
            if (!string.IsNullOrEmpty(dto.Name)) { thisCoach.Name = dto.Name; }
            if (!string.IsNullOrEmpty(dto.AvatarUrl)) { thisCoach.AvatarUrl = dto.AvatarUrl; }
            if (!string.IsNullOrEmpty(dto.Introduction)) { thisCoach.Introduction = dto.Introduction; }
            if (dto.DistrictId.HasValue) { thisCoach.DistrictId = dto.DistrictId; }

            thisCoach.UpdatedAt = DateTime.Now;

            // 3. 存檔：這時候 EF 就會知道 thisCoach 被動過了，發出 UPDATE 指令
            await _context.SaveChangesAsync();

            // 4. 回傳：因為有只放id的欄位 所以要回傳抓新的對應名字
            var resultData = await _context.ExpCoaches
                .Where(c => c.Id == thisCoach.Id)
                .Select(c => new DCoachEdit
                {
                    Name = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    Introduction = c.Introduction,
                    DistrictId = c.DistrictId,
                    DistrictName = c.District.Name
                }).FirstOrDefaultAsync();

            return new DAPIResponse<DCoachEdit> { IsSuccess = true, Message = "更新成功！教練大人進化了！", Data = resultData };

        }
        #endregion

        #region 系統推薦教練  
        public async Task<List<DCoachRecommend>> CoachRecommand(int thisCoachId)
        {
            // 1. 先把「當前教練」的地區與專長抓出來（這是我們的基準點）
            var currentCoachInfo = await _context.ExpCoaches
                .Where(c => c.Id == thisCoachId)
                .Select(c => new
                {
                    District = c.DistrictId,
                    // 抓出他所有的專長 ID
                    SpecialityIds = c.Specialities.Select(s => s.SportsName).ToList()
                })
                .FirstOrDefaultAsync();

            if (currentCoachInfo == null) return new List<DCoachRecommend>();
            // 2. 開始找「臭味相投」的教練
            var query = _context.ExpCoaches
                .Where(c => c.Id != thisCoachId) // 排除自己
                .Where(c => c.DistrictId == currentCoachInfo.District &&
                            c.Specialities.Any(s => currentCoachInfo.SpecialityIds.Contains(s.SportsName)));
            // 3. 執行隨機排序並取前 3 名
            // 提示：在 LINQ to Entities 中，Guid.NewGuid() 常用來模擬隨機排序//
            var recommendedList = await query
                .OrderBy(c => Guid.NewGuid())
                .Take(3)
                .Select(c => new DCoachRecommend
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    // 這裡假設你有關聯到 TripDistricts
                    District = c.TripDistricts.Select(d => d.Name).ToList(),
                    Specialities = c.Specialities
                                    .Select(s => s.SportsName)
                                    .ToList()
                })
                .ToListAsync();
            //+排除自己

            return recommendedList;
        }
        #endregion

        #region 收藏清單
        public async Task<List<DCoachFavList>> GetMyFavCoach(int userId)
        {
            // 1. 先拿到 Entity 列表
            var fav = _context.ExpFavorites
                .Where(f => f.UserId == userId)
                .Select(f => new DCoachFavList
                {
                    UserId = f.UserId,
                    CoachId = f.CoachId,
                    CoachName = f.Coach.Name, // 透過導覽屬性抓教練名
                    AvatarUrl = f.Coach.AvatarUrl,
                    District = f.Coach.TripDistricts.Select(d => d.Name).ToList(),
                    Specialities = f.Coach.Specialities.Select(sm => sm.SportsName).ToList(),
                    AvgRating = f.Coach.ExpReviews.Any()
                        ? Math.Round( f.Coach.ExpReviews.Average(r => (double)r.Rating), 1)
                        : 0,
                    ReviewCount = f.Coach.ExpReviews.Count()
                }).ToListAsync();

            return await fav;
        }
        #endregion

        #endregion

        #region~~課程~~
        #region 課程模板建立
        public async Task<DAPIResponse<string>> CreateTemplate(DCourseCreate dto, int userId)
        {
            try {
                var coachExists = await _context.ExpCoaches
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (coachExists == null)
                    return new DAPIResponse<string> { IsSuccess = false,Message = "教練不存在"};
                
                //主表圖表分開處理
                var t = new ExpCourseTemplate
            {
                CoachId = coachExists.Id,
                Title = dto.Title,
                Description = dto.Description,
                Difficulty = dto.Difficulty,
                Price = dto.Price,
                Location = dto.Location,
                CreatedAt = DateTime.Now
            };
            //有傳圖片再做這步
            if (dto.PhotoUrls != null && dto.PhotoUrls.Any())
            {
                foreach (var pic in dto.PhotoUrls)
                {
                        if (string.IsNullOrWhiteSpace(pic)) continue;
                        t.ExpCoursePhotos.Add(new ExpCoursePhoto {
                                                PhotoUrl = pic,
                                                UploadedAt = DateTime.Now
                                            });
                }
            }
            //save--EF 會幫你動用 Transaction，確保模板跟照片「同時成功」或「同時失敗」)
            _context.ExpCourseTemplates.Add(t);
            //update
            await _context.SaveChangesAsync();

            return new DAPIResponse<string>
            {
                IsSuccess = true,
                Message = "新課程模板做好啦 ！"
            }; 
            
            }
            catch (Exception ex) { throw new Exception(ex.InnerException?.Message); }


            
        }
        #endregion


        #region 課程選時間上架 
        //TODO
        public async Task<DAPIResponse<string>> OpenSession(DCourseOpenSession dto, int TemplateId, int currentUserId)
        {
            //找有沒有模板
            var t = await _context.ExpCourseTemplates.FirstOrDefaultAsync(t => t.Id == TemplateId);
            if (t == null) { throw new Exception("凡人不能看的天書"); }
            //使用者和教練對得上
            var coach = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.Id == t.CoachId);
            if (coach == null || coach.UserId != currentUserId)
                return new DAPIResponse<string> { IsSuccess = false, Message = "你沒有權限為此課程開課喔！" };


            //選時間和人數
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly day60 = today.AddDays(60);

            foreach (var date in dto.SelectedDates)
            {

                if (date < today || date > day60) continue;
                //找有沒有衝堂
                bool isConflict = await _context.ExpCourseSessions.AnyAsync(s =>
                    s.CoachId == t.CoachId &&
                    s.SessionDate == date &&
                    s.TimeSlot == dto.TimeSlot);
                if (isConflict) throw new Exception("尚未習得隱分身之術 你逆");

                var newSession = new ExpCourseSession
                {
                    CourseTemplateId = TemplateId,
                    CoachId = t.CoachId,
                    SessionDate = date,
                    TimeSlot = dto.TimeSlot,
                    MaxParticipants = dto.MaxStudents,
                    CurrentParticipants = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                 _context.ExpCourseSessions.AddAsync(newSession);
            }
            await _context.SaveChangesAsync();
            return new DTO.DAPIResponse<string>
            {
                IsSuccess = true,
                Message = "課程開放報名~"
            };

        }
        #endregion

        #region 課程展示
        //TODO
        public async Task<DCourseOpenSession> ThisCourse(int coachId, int courseId)
        {
            var result = await _context.ExpCourseSessions
                    .Where(c => c.CoachId == coachId && c.Id == courseId)
                    .Select(c => new DCourseOpenSession
                    {
                       // TemplateId = c.Id,
                        //CoachId = c.CoachId,
                        TimeSlot = c.TimeSlot,
                        MaxStudents = c.MaxParticipants,
                        SelectedDates = new List<DateOnly> { c.SessionDate.Value },
                       // UpdatedAt = c.UpdatedAt
                    }).FirstOrDefaultAsync();
            return result;
        }
        #endregion
        #region 課程模板編輯
        public async Task<DCourseEdit> EditTemplate(DCourseEdit dto, int TemplateId)
        {
            var t = _context.ExpCourseTemplates
                .Include(x => x.ExpCoursePhotos)
                .FirstOrDefault(c => c.Id == TemplateId);
            if (t == null) return null;

            if (!string.IsNullOrEmpty(dto.Title)) { t.Title = dto.Title; }
            if (!string.IsNullOrEmpty(dto.Description)) { t.Description = dto.Description; }
            if (!string.IsNullOrEmpty(dto.Difficulty)) { t.Difficulty = dto.Difficulty; }
            if (dto.Price > 0) { t.Price = dto.Price; }
            if (!string.IsNullOrEmpty(dto.Location)) { t.Location = dto.Location; }
            if (dto.PhotoUrls != null)
            {
                // 1. 取得資料庫目前的照片網址清單

                var originPhoto = t.ExpCoursePhotos.ToList();
                var existingUrls = originPhoto.Select(p => p.PhotoUrl).ToList();

                // 2. 找出「要刪除」的照片 (情境 2 & 4)
                // 那些在資料庫裡，但不在前端名單中的
                var photosToRemove = originPhoto
                    .Where(p => !dto.PhotoUrls.Contains(p.PhotoUrl))
                    .ToList();

                foreach (var p in photosToRemove)
                {
                    _context.ExpCoursePhotos.Remove(p);
                }

                // 3. 找出「要新增」的照片 (情境 1 & 3 & 4)
                // 那些在前端名單中，但不在資料庫裡的
                var urlsToAdd = dto.PhotoUrls
                    .Where(url => !existingUrls.Contains(url))
                    .ToList();

                foreach (var url in urlsToAdd)
                {
                    t.ExpCoursePhotos.Add(new ExpCoursePhoto
                    {
                        PhotoUrl = url,
                        UploadedAt = DateTime.Now
                        // course_template_id 會由 EF Core 自動幫你關聯，不用手填！
                    });
                }
            }
            t.UpdatedAt = DateTime.Now;


            await _context.SaveChangesAsync();
            return dto;
        }
        #endregion
        #endregion

        #region ~~評論~~
        #region 新增評論
        public async Task<DAPIResponse<string>> CreateReview(DReview dto, int userId, int courseOId)
        {
            //先去「訂單表 (ExpCourseOrders)」確認有沒有這筆訂單，順便把 CoachId 拿回來
            var order = await _context.ExpCourseOrders
                .Where(o => o.Id == courseOId && o.UserId == userId)
                .Select(o => new {
                    CoachId = o.CourseSession.CoachId
                }).FirstOrDefaultAsync();
            if (order == null) { 
                return new DAPIResponse<string> { IsSuccess = false, Message ="沒有課程可以評論" };}

            var newReview = new ExpReview
            {
                UserId = userId,
                CoachId = order.CoachId, // 從訂單帶入教練 ID
                CourseOrderId = courseOId,
                Rating = dto.Rating,
                ReviewContent = dto.ReviewContent,
                ReviewedAt = DateTime.Now, // 記得加上評論時間
                IsHidden = false
            };
            await _context.ExpReviews.AddAsync(newReview);
            await _context.SaveChangesAsync();
            return new DAPIResponse<string>
            {
                IsSuccess = true,
                Message = "~感謝大大撥冗評論~"
            };
        }
        #endregion
        #endregion




        #region 評論
        #region 新增評論
        #endregion
        #region 編輯評論
        #endregion
        #region 刪除評論
        #endregion
        #endregion

        #region 交易流程
        #region 預約課程
        #endregion
        #region 支付 
        #endregion
        #region 歷史交易紀錄 
        #endregion
        #endregion

        #region 營運 
        #endregion

        #region 課程--搜尋
        #region 課程搜尋-有名額
        #endregion
        #region 課程搜尋-難度
        #endregion
        #endregion

    }
}
