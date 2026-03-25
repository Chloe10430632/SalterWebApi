using Azure;
using CloudinaryDotNet.Actions;
using ExpServiceHelper.DTO;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using FluentEcpay;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalterEFModels.EFModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Collections.Specialized.BitVector32;
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
        private readonly SPhoto _sPhoto;
        public SCoachMethods(SalterDbContext dbContext, SPhoto sPhoto) { _context = dbContext; _sPhoto = sPhoto; }
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

        #region 申請加入教練(新增)  
            public async Task<DAPIResponse<int>> CreateCoach(DCoachEdit dto, int currentUserId)
            {
                // 1. 檢查是否已經是教練（協作規範：一人只能有一個教練身份）
                bool exists = await _context.ExpCoaches.AnyAsync(c => c.UserId == currentUserId);
                if (exists) return new DAPIResponse<int> { IsSuccess = false, Message = "您已經是教練囉！" };

            //處理照片
            string upLoadUrl = "default_avatar_url";
            string upLoadPublicId = null;

            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                //呼叫SPhoto
                var p = await _sPhoto.AddPhotoAsync(new List<IFormFile> { dto.AvatarFile });
                if (  p != null && p.Any()) {
                    var result = p.First();
                    upLoadUrl = result.SecureUrl.ToString();
                    upLoadPublicId = result.PublicId;
                }
                else{
                    return new DAPIResponse<int> { IsSuccess = false, Message = "圖片上傳失敗" };
                }
            }

            //  建立新實體
            var newCoach = new ExpCoach
            {
                UserId = currentUserId, // 綁定目前的 User
                Name = dto.Name,
                AvatarUrl = upLoadUrl,
                PublicId = upLoadPublicId,
                Introduction = dto.Introduction,
                CityId = dto.CityId,
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

            //處理圖片
            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                // A. 刪除舊圖片 (建議在資料表增加一個 PublicId 欄位，否則需從 URL 解析)
                if (!string.IsNullOrEmpty(thisCoach.AvatarUrl))
                {
                    //string oldPublicId = ExtractPublicIdFromUrl(thisCoach.AvatarUrl);
                    await _sPhoto.DeletePhotoAsync(new List<string> { dto.PublicId});
                }

                // B. 上傳新圖片
                var uploadResults = await _sPhoto.AddPhotoAsync(new List<IFormFile> { dto.AvatarFile });
                var uploadResult = uploadResults.FirstOrDefault();

                if (uploadResult != null && uploadResult.Error == null) {
                    thisCoach.AvatarUrl = uploadResult.SecureUrl.AbsoluteUri;
                }
                else {return new DAPIResponse<DCoachEdit> { IsSuccess = false, Message = "圖片上傳失敗" };
                }
            }

            // 2.(賦值)：把前端傳來的 dto 資料塞進資料庫的 entity 裡
            // 這一步才是真正的「更新」！
            if (!string.IsNullOrEmpty(dto.Name)) { thisCoach.Name = dto.Name; }
            if (!string.IsNullOrEmpty(dto.Introduction)) { thisCoach.Introduction = dto.Introduction; }
            if (dto.DistrictId.HasValue) { thisCoach.DistrictId = dto.DistrictId; }

            thisCoach.UpdatedAt = DateTime.Now;

            // 3. 存檔：這時候 EF 就會知道 thisCoach 被動過了，發出 UPDATE 指令
            await _context.SaveChangesAsync();

            return new DAPIResponse<DCoachEdit> {
                IsSuccess = true, Message = "更新成功！教練大人進化了！" };
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
        public async Task<DAPIResponse<DCourseCreate>> CreateTemplate(DCourseCreate dto, int userId)
        {
            try
            {
                var coachExists = await _context.ExpCoaches
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (coachExists == null)
                    return new DAPIResponse<DCourseCreate> { IsSuccess = false, Message = "教練不存在" };

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
                    // 呼叫你的 SPhoto Service 進行異步上傳
                    // 建議在 Service 注入 SPhoto _sPhoto;
                    var uploadResults = await _sPhoto.AddPhotoAsync(dto.PhotoUrls);

                    // 建立一個空的清單給導覽屬性
                    t.ExpCoursePhotos = new List<ExpCoursePhoto>();

                    foreach (var result in uploadResults)
                    {
                        if (result.Error != null) continue; // 或是處理錯誤

                        var newPhoto = new ExpCoursePhoto
                        {
                            PhotoUrl = result.SecureUrl.ToString(),
                            PublicId = result.PublicId,
                            UploadedAt = DateTime.Now
                            // 重點：不要寫 Id = 0 或任何東西
                        };

                        t.ExpCoursePhotos.Add(newPhoto);
                    }
                }
                    //save--EF 會幫你動用 Transaction，確保模板跟照片「同時成功」或「同時失敗」)
                    _context.ExpCourseTemplates.Add(t);
                    //update
                    await _context.SaveChangesAsync();

                    return new DAPIResponse<DCourseCreate>
                    {
                        IsSuccess = true,
                        Message = "新課程模板做好啦 ！",
                        Data = dto
                    };

                
            }
            catch (Exception ex) { throw new Exception(ex.InnerException?.Message); }


            
        }
        #endregion

        #region 課程模板編輯
        public async Task<DAPIResponse<DCourseTempEdit>> EditTemplate(DCourseTempEdit dto, int TemplateId, int currentUserId)
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
            if (dto.ExistingPhotosJson != null)
            {
                // --- A. 處理新圖片上傳 ---
                if (dto.NewImageFiles != null && dto.NewImageFiles.Any())
                {
                    // 呼叫你寫好的 SPhoto
                    var uploadResults = await _sPhoto.AddPhotoAsync(dto.NewImageFiles);

                    foreach (var res in uploadResults)
                    {
                        t.ExpCoursePhotos.Add(new ExpCoursePhoto
                        {
                            PhotoUrl = res.SecureUrl.ToString(), // Cloudinary 回傳的網址
                            PublicId = res.PublicId,             // 務必存下這個 ID！
                            UploadedAt = DateTime.Now
                        });
                    }
                }

                // --- B. 處理舊圖片刪除 ---
                // 找出「原本在資料庫」但「不在前端傳回的 ExistingPhotos」中的照片
                if (!string.IsNullOrEmpty(dto.ExistingPhotosJson))
                {
                    // 1. 先把 JSON 字串變成 C# 的小盒子清單 (List<DPhoto>)
                    var existingPhotos = System.Text.Json.JsonSerializer.Deserialize<List<DPhoto>>(
                        dto.ExistingPhotosJson,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (existingPhotos != null)
                    {
                        // 2. 拿到「想要保留」的 ID 清單 (例如：["id1", "id2"])
                        var keepPublicIds = existingPhotos.Select(p => p.PublicId).ToList();

                        // 3. 【宣告 photosToRemove】：
                        // 找出「原本在資料庫」但「不在保留名單內」的照片
                        //只針對「已經有 Id」且「已經在資料庫」的照片進行比對
                        var photosToRemove = t.ExpCoursePhotos
                            .Where(p => p.Id != 0) // 👈 關鍵！只抓已經存在資料庫的老照片，避開剛 Add 進去的新照片
                            .Where(p => !keepPublicIds.Contains(p.PublicId) && !string.IsNullOrEmpty(p.PublicId))
                            .ToList();

                        // 4. 如果真的有要刪除的，才動手
                        if (photosToRemove.Any())
                        {
                            // (1) 從 Cloudinary 刪除實體檔案
                            var idsToDelete = photosToRemove.Select(p => p.PublicId!).ToList();
                            await _sPhoto.DeletePhotoAsync(idsToDelete);

                            // (2) 從資料庫 EF 追蹤中移除（標記為 Deleted）
                            _context.ExpCoursePhotos.RemoveRange(photosToRemove);
                        }
                    }
                } 
               
              
            }
            t.UpdatedAt = DateTime.Now;


            await _context.SaveChangesAsync();
            return new DTO.DAPIResponse<DCourseTempEdit> { IsSuccess = true, Message = "修改成功", Data =dto };
        }
        #endregion

        #region 課程選時間上架 
        public async Task<DAPIResponse<DCourseOpenSession>> OpenSession(DCourseOpenSession dto, int TemplateId, int currentUserId)
        {
            //找有沒有模板
            var t = await _context.ExpCourseTemplates.FirstOrDefaultAsync(t => t.Id == TemplateId);
            if (t == null) { throw new Exception("凡人不能看的天書"); }
            //使用者和教練對得上
            var coach = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.Id == t.CoachId);
            if (coach == null || coach.UserId != currentUserId)
                return new DTO.DAPIResponse<DCourseOpenSession> { IsSuccess = false, Message = "冒牌教練!"};


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
                await _context.ExpCourseSessions.AddAsync(newSession);
            }
            await _context.SaveChangesAsync();
            return new DTO.DAPIResponse<DCourseOpenSession>
            {
                IsSuccess = true,
                Message = "課程開放報名~",
                Data = dto
            };

        }
        #endregion

        #region 課程時段刪除
        public async Task<DAPIResponse<string>> DeleteCourseSession(int courseSessionId, int currentUserId)
        {
            var session = await _context.ExpCourseSessions.FirstOrDefaultAsync(s => s.Id == courseSessionId);
            if (session == null)
                throw new Exception("沒有對應的資料");

            var c = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.UserId == currentUserId);
            if (c == null || session.CoachId != c.Id)
                return new DAPIResponse<string> { IsSuccess = false, Message = "權限不足，您只能刪除自己的課程" };

            if (session.CurrentParticipants > 0)
                return new DAPIResponse<string>
                {
                    IsSuccess = false,
                    Message = "有學生報名了，不能耍任性"
                };

            _context.ExpCourseSessions.Remove(session);
            await _context.SaveChangesAsync();
            return new DAPIResponse<string> { IsSuccess = true, Message = "成功刪除時段" };

        }

        #endregion

        #region 課程展示介紹
        public async Task<DAPIResponse<DCourseInfo>> ThisCourse(int courseId)
        {
            var courseSession = await _context.ExpCourseSessions
                                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (courseSession == null) return new DAPIResponse<DCourseInfo>{ IsSuccess = false, Message = "找不到該課程資訊"};

            var result = await _context.ExpCourseSessions
                    .Where(c => c.Id == courseId)
                    .Select(c => new DCourseInfo
                    {
                        CoachId = c.CoachId,
                        SelectedDates = c.SessionDate.HasValue
                                ? new List<DateOnly> { c.SessionDate.Value }
                                : new List<DateOnly>(),
                        TimeSlot = c.TimeSlot,
                        MaxStudents = c.MaxParticipants,
                        UpdatedAt = c.UpdatedAt
                    }).FirstOrDefaultAsync();

            if (result == null){
                return new DAPIResponse<DCourseInfo>{
                    IsSuccess = false,
                    Message = "找不到該課程資訊"
                };
            }

            return new DTO.DAPIResponse<DCourseInfo>
            {
                IsSuccess = true,
                Message = "課程展示中",
                Data = result
            };
        }
        #endregion
        #endregion

        #region ~~評論~~
        #region 新增評論
        public async Task<DAPIResponse<string>> CreateReview(DReview dto,int userId, int courseOId)
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

        #region 編輯評論
        public async Task<DAPIResponse<DReview>> EditReview(DReview dto, int userId, int courseId, int reviewId) {
            var r = await _context.ExpReviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if (r == null) return new DAPIResponse<DReview> { IsSuccess = false, Message = "找不倒評論" };

            //進行修改：直接把 DTO 的資料倒進去 Entity(原本的那筆)
            r.ReviewContent = dto.ReviewContent;
            r.Rating = dto.Rating;
            r.UpdateAt = DateTime.Now;
            
          
            await _context.SaveChangesAsync();
            return new DAPIResponse<DReview>
            {
                IsSuccess = true,
                Message = $"更新成功，Reviewed at {userId}",
                Data = dto
            };
        }
        #endregion

        #region 刪除評論
        public async Task<DAPIResponse<string>> DeleteReview(int userId, int reviewId) { 
            var review = await _context.ExpReviews.FirstOrDefaultAsync( r => r.Id == reviewId && r.UserId == userId);
            if (review == null)
                throw new Exception("沒有對應的資料");
            _context.ExpReviews.Remove(review);
            await _context.SaveChangesAsync();
            return new DAPIResponse<string> { IsSuccess = true, Message = "成功刪除評論" };
        }
        #endregion

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
        #endregion

        #region 交易流程
        #region 新增預約課程
        public async Task<DAPIResponse<string>> CourseReserve(DCourseOrder dto, int userId, int courseSessionId)
        {
           //找這堂課，順便拿教練ID
            var session = await _context.ExpCourseSessions
                          .FirstOrDefaultAsync( r => r.Id == dto.CourseSessionId );
            if (session == null) {
                return new DAPIResponse<string> { IsSuccess = false, Message = "沒有課程可以評論" };
            }
            //核對課堂名額
            if (session.CurrentParticipants >= session.MaxParticipants) {
                return new DAPIResponse<string> { IsSuccess = false, Message = "額滿了 你晚了一步QQ" };
            }

            //建立一筆 ExpTransaction，取得 TransactionId
            decimal? coursePrice = session.CourseTemplate.Price;
            var transac = new ExpTransaction {
                SenderUserId = userId,
                ReceiveUserId = session.CoachId, // 從場次拿教練 ID
                Amount = coursePrice,            // 假設金額，之後可從 Template 抓
                Status = 0,                      // 0: 已建立/待付款
                TypeId = 3,                     
                CreatedAt = DateTime.Now
            };
            await _context.ExpTransactions.AddAsync( transac );
            await _context.SaveChangesAsync();

            //建立預約實體 (Entity)
            var reserve = new ExpCourseOrder {
                UserId = userId,
                CourseSessionId = courseSessionId,
                ReservedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ExpTransactionId = transac.Id,
                Status = 0
            };
            //課程報名人+1
            session.CurrentParticipants += 1;
           
            await _context.ExpCourseOrders.AddAsync(reserve);
            await _context.SaveChangesAsync();
            return new DAPIResponse<string> {
              IsSuccess = true,
              Message = "預約成功，請繳費完成報名",
              Data = reserve.ToString()
            };
        }
        #endregion
        #region 歷史交易紀錄 
        public async Task<List<DTransac>> TransacList(int userId) {
            using (var db = new SalterDbContext()) {
                var history = await db.ExpTransactions
                            .Where(h => h.SenderUserId == userId)
                            .Select(h => new DTransac { 
                                TransactionId = h.Id,
                                CourseName = h.ExpCourseOrders.FirstOrDefault().CourseSession.CourseTemplate.Title,
                                CoaId = h.ExpCourseOrders.FirstOrDefault().CourseSession.CoachId,
                                Amount = h.Amount,
                                Status = h.Status ==0 ? "等待付款" :
                                        h.Status == 1 ? "交易成功" : "已取消",
                                OrderDate = h.CreatedAt
                            })
                            .OrderByDescending(h => h.OrderDate)
                            .ToListAsync();
                return  history;
            }
        
        }
        #endregion

        #region 結帳 
        //用套件 + 前台回傳結帳需求時的邏輯
        //在SECPay
        #endregion
        #endregion



        #region 交易流程

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
