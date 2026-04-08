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
using TripRepositoryHelper.IRepository;
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
                District = c.District != null ? c.District.Name : "未設定",
                DistrictId = c.DistrictId,
                CityId = c.CityId,
                Specialities = c.ExpCoachSpeciallityMappings.Select(s => s.Specialities.SportsName).ToList(),
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
        private readonly ITripRepository _tr;

        public SCoachMethods(SalterDbContext dbContext, SPhoto sPhoto, ITripRepository tr) {
            _context = dbContext; _sPhoto = sPhoto; _tr = tr; }
        #endregion

        #region 入口
        #region~~搜尋-名字~~
        public async Task<List<DCoachInfo>> GetCoachName(string key)
        {
            var result = await _context.ExpCoaches
                .Where(c => c.Name.Contains(key))
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
                 .Where(c => c.ExpCoachSpeciallityMappings.Any(s => s.Specialities.SportsName.Contains(key)))
                 .SelectCoachInfo()
                 .ToListAsync();
            return result;
        }
        #endregion

        #region~~排序-熱門~~
        public async Task<List<DCoachInfo>> CoachPopular(int page, int pageSize)
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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Entity); // 【關鍵】排序完後，我們只要回傳教練實體

            // 2. 既然拿到了「前12名的教練實體」，直接接上你的擴充方法！
            return await rankedCoachesQuery.SelectCoachInfo().ToListAsync();
        }
        #endregion

        #region~~排序-最新~~
        public async Task<List<DCoachInfo>> GetCoachNewest(int page, int pageSize)
        {
            var result = await _context.ExpCoaches
                 .OrderByDescending(c => c.CreatedAt)
                 .Skip((page - 1) * pageSize)
                 .Take(pageSize)
                 .SelectCoachInfo()
                 .ToListAsync();
            return result;
        }
        #endregion
        #endregion

        #region~~教練~~

        #region 申請加入教練(新增)  
        public async Task<DCoachEdit> CreateCoach(DCoachEdit dto, int currentUserId)
        {
            // 1. 檢查是否已經是教練（協作規範：一人只能有一個教練身份）
            bool exists = await _context.ExpCoaches.AnyAsync(c => c.UserId == currentUserId);
            if (exists) return new DCoachEdit();

            //處理照片
            string upLoadUrl = "default_avatar_url";
            string upLoadPublicId = null;

            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                //呼叫SPhoto
                var p = await _sPhoto.AddPhotoAsync(new List<IFormFile> { dto.AvatarFile });

                if (p != null && p.Any())
                {
                    var result = p.First();
                    upLoadUrl = result.SecureUrl.ToString();
                    upLoadPublicId = result.PublicId;
                }
                else
                {
                    throw new InvalidOperationException("圖存不進去");

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
                DistrictId = dto.DistrictId,
                CityId = dto.CityId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsSuspended = false // 預設不停權
            };

            // 把教練加到 Context 中 (這時候 EF 已經開始追蹤這個 newCoach 了)
            _context.ExpCoaches.Add(newCoach);

            //  處理專業 (不需要手動寫 CoachId！)
            if (dto.SpecialityIds != null && dto.SpecialityIds.Any())
            {
                foreach (var specId in dto.SpecialityIds)
                {
                    // 直接 new 出 Mapping 資料，CoachId 先空著
                    // 關鍵在於：只要你把這些 Mapping 加進 Context，EF 存檔時會自動補上 Id
                    _context.ExpCoachSpeciallityMappings.Add(new ExpCoachSpeciallityMapping
                    {
                        // 💡 這裡直接指向剛才 new 出來的物件
                        Coach = newCoach,
                        SpecialitiesId = specId
                    });
                }
            }
            await _context.SaveChangesAsync();
            dto.CoachId = newCoach.Id;

            return dto;
        }
        #endregion  

        #region 自己自介 
        public async Task<DCoachInfo> MyCoachInfo(int userId) {
            var q = _context.ExpCoaches
                .Where(c => c.UserId == userId)
                .Select(c => new DCoachInfo
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = c.District != null ? c.District.Name : "未設定",
                    DistrictId = c.DistrictId,
                    CityId = c.District != null ? c.District.CityId : (int?)null,
                    AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                    ReviewCount = c.ExpReviews.Count(),
                    Specialities = c.ExpCoachSpeciallityMappings.Select(s => s.Specialities.SportsName).ToList(),
                    Introduction = c.Introduction,
                    CreatedAt = c.CreatedAt
                });
            var result = await q.FirstOrDefaultAsync();
            return result;
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
                         District = c.District != null ? c.District.Name : "未設定",
                         DistrictId = c.DistrictId,
                         CityId = c.District != null ? c.District.CityId : (int?)null,
                         AvgRating = c.ExpReviews.Any() ? Math.Round(c.ExpReviews.Average(r => (double)r.Rating), 1) : 0,
                         ReviewCount = c.ExpReviews.Count(),
                         Specialities = c.ExpCoachSpeciallityMappings.Select(s => s.Specialities.SportsName).ToList(),
                         Introduction = c.Introduction,
                         CreatedAt = c.CreatedAt

                     });

            // 關鍵在這裡！使用 FirstOrDefaultAsync() 真正去資料庫拿「第一筆或預設值」
            var result = await query.FirstOrDefaultAsync();
            return result;
        }
        #endregion

        #region 教練編輯 
        public async Task<DCoachEdit> EditCoachInfo(DCoachEdit dto, int currentUserId)
        {
            // 除了找 Coach ID，還要確認 user_id 也是本人
            var thisCoach = await _context.ExpCoaches
                .FirstOrDefaultAsync(c => c.UserId == currentUserId);

            if (thisCoach == null) throw new InvalidOperationException("教練不匹配");

            if (dto.SpecialityIds != null)
            {
                // 1. 先把該教練舊的專業全部刪除 (砍掉舊的連結)
                // 假設你的 Mapping 表叫 ExpCoachSpecialties
                var oldMappings = _context.ExpCoachSpeciallityMappings.Where(s => s.CoachId == thisCoach.Id);
                _context.ExpCoachSpeciallityMappings.RemoveRange(oldMappings);

                // 2. 根據前端傳來的 ID 列表，重新建立新的連結
                    foreach (var specId in dto.SpecialityIds)
                    {
                        _context.ExpCoachSpeciallityMappings.Add(new ExpCoachSpeciallityMapping
                        {
                            CoachId = thisCoach.Id,
                            SpecialitiesId = specId 
                        });
                    }
                
            }

            //處理圖片
            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                // A. 刪除舊圖片 (建議在資料表增加一個 PublicId 欄位，否則需從 URL 解析)
                if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
                {
                    // 用資料庫裡的舊 PublicId 刪除
                    if (!string.IsNullOrEmpty(thisCoach.PublicId))
                    {
                        await _sPhoto.DeletePhotoAsync(new List<string> { thisCoach.PublicId });
                    }

                    var uploadResults = await _sPhoto.AddPhotoAsync(new List<IFormFile> { dto.AvatarFile });
                    var uploadResult = uploadResults.FirstOrDefault();

                    if (uploadResult != null && uploadResult.Error == null)
                    {
                        thisCoach.AvatarUrl = uploadResult.SecureUrl.AbsoluteUri;
                        thisCoach.PublicId = uploadResult.PublicId; // ← 記得更新新的 PublicId
                    }
                    else
                    {
                        throw new InvalidOperationException("圖存不進去");
                    }
                }
            }

            // 2.(賦值)：把前端傳來的 dto 資料塞進資料庫的 entity 裡
            // 這一步才是真正的「更新」！
            if (!string.IsNullOrEmpty(dto.Name)) { thisCoach.Name = dto.Name; }
            if (!string.IsNullOrEmpty(dto.Introduction)) { thisCoach.Introduction = dto.Introduction; }
            if (dto.CityId.HasValue) { thisCoach.CityId = dto.CityId; }
            if (dto.DistrictId.HasValue) { thisCoach.DistrictId = dto.DistrictId; }

            thisCoach.UpdatedAt = DateTime.Now;

            // 3. 存檔：這時候 EF 就會知道 thisCoach 被動過了，發出 UPDATE 指令
            await _context.SaveChangesAsync();

            return dto;
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
                    SpecialityIds = c.ExpCoachSpeciallityMappings.Select(s => s.Specialities.SportsName).ToList()
                })
                .FirstOrDefaultAsync();

            if (currentCoachInfo == null) return new List<DCoachRecommend>();
            // 2. 開始找「臭味相投」的教練
            var query = _context.ExpCoaches
                .Where(c => c.Id != thisCoachId) // 排除自己
                .Where(c => c.DistrictId == currentCoachInfo.District  ||
                            c.ExpCoachSpeciallityMappings.Any(s => currentCoachInfo.SpecialityIds.Contains(s.Specialities.SportsName)));
            // 3. 執行隨機排序並取前 2 名
            // 提示：在 LINQ to Entities 中，Guid.NewGuid() 常用來模擬隨機排序//
            var recommendedList = await query
                .OrderBy(c => Guid.NewGuid())
                .Take(2)
                .Select(c => new DCoachRecommend
                {
                    CoachId = c.Id,
                    CoachName = c.Name,
                    AvatarUrl = c.AvatarUrl,
                    District = c.District.Name,
                    Specialities = c.ExpCoachSpeciallityMappings.Select(s => s.Specialities.SportsName)
                                    .ToList()
                })
                .ToListAsync();

            if (recommendedList.Count == 0)
            {
                recommendedList = await _context.ExpCoaches
                    .Where(c => c.Id != thisCoachId)
                    .OrderBy(c => Guid.NewGuid())
                    .Take(2)
                    .Select(c => new DCoachRecommend
                    {
                        CoachId = c.Id,
                        CoachName = c.Name,
                        AvatarUrl = c.AvatarUrl,
                        District = c.District.Name,
                        Specialities = c.ExpCoachSpeciallityMappings.Select(s => s.Specialities.SportsName)
                                    .ToList()
                    }).ToListAsync();
            }

            return recommendedList;
        }
        #endregion

        #endregion

        #region 查看收藏(保持愛心) 
        public async Task<List<int>> HeartIds(int userId)
        {
            return await _context.ExpFavorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.CoachId).ToListAsync();
        }
        #endregion

        #region 收藏清單
        public async Task<List<DCoachFavList>> GetMyFavCoach(int currentUserId, int page, int pageSize)
        {
            // 1. 先拿到 Entity 列表
            var fav = _context.ExpFavorites
                .Where(f => f.UserId == currentUserId)
                .Select(f => new DCoachFavList
                {
                    UserId = f.UserId,
                    CoachId = f.CoachId,
                    CoachName = f.Coach.Name, // 透過導覽屬性抓教練名
                    AvatarUrl = f.Coach.AvatarUrl,
                    District = f.Coach.TripDistricts.Select(d => d.Name).ToList(),
                    Specialities = f.Coach.ExpCoachSpeciallityMappings.Select(sm => sm.Specialities.SportsName).ToList(),
                    AvgRating = f.Coach.ExpReviews.Any()
                        ? Math.Round(f.Coach.ExpReviews.Average(r => (double)r.Rating), 1)
                        : 0,
                    ReviewCount = f.Coach.ExpReviews.Count(),
                    FavoritedAt = f.FavoritedAt
                })
                .OrderByDescending(f => f.FavoritedAt) // 依照收藏時間排序，最新的在前面
               .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return await fav;
        }
        #endregion

        #region~~課程~~
        #region 課程模板建立
        public async Task<DCourseCreate> CreateTemplate(DCourseCreate dto, int userId)
        {
            try
            {
                var coachExists = await _context.ExpCoaches
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (coachExists == null) throw new InvalidOperationException("教練不匹配");

                //主表圖表分開處理
                var t = new ExpCourseTemplate
                {
                    CoachId = coachExists.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    Difficulty = dto.Difficulty,
                    Price = dto.Price,
                    Location = dto.Location,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
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

                return dto;
            }
            catch (Exception ex) { throw new Exception(ex.InnerException?.Message); }



        }
        #endregion

        #region 課程模板編輯
        public async Task<DCourseTempEdit> EditTemplate(DCourseTempEdit dto, int TemplateId, int currentUserId)
        {
            var t = _context.ExpCourseTemplates
                .Include(x => x.ExpCoursePhotos)
                .FirstOrDefault(c => c.Id == TemplateId);
            if (t == null) throw new InvalidOperationException("模板不匹配");


            if (!string.IsNullOrEmpty(dto.Title)) { t.Title = dto.Title; }
            if (!string.IsNullOrEmpty(dto.Description)) { t.Description = dto.Description; }
            if (!string.IsNullOrEmpty(dto.Difficulty)) { t.Difficulty = dto.Difficulty; }
            if (dto.Price > 0) { t.Price = dto.Price; }
            if (!string.IsNullOrEmpty(dto.Location)) { t.Location = dto.Location; }
            if (dto.LocationData != null && !string.IsNullOrEmpty(dto.LocationData.GooglePlaceId))
            {
                t.LocationId = await EnsureLocationExistsAsync(dto.LocationData);
                t.Location = dto.LocationData.LocationName;
            }
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

                    if (existingPhotos != null && existingPhotos.Any())
                    {
                        // 2. 拿到「想要保留」的 ID 清單 (例如：["id1", "id2"])
                        var keepPublicIds = existingPhotos.Select(p => p.PublicId).Where(id => !string.IsNullOrEmpty(id)).ToList();

                        // 3. 【宣告 photosToRemove】：
                        // 找出「原本在資料庫」但「不在保留名單內」的照片
                        //只針對「已經有 Id」且「已經在資料庫」的照片進行比對
                        var photosToRemove = t.ExpCoursePhotos
                            .Where(p => p.Id != 0 && !string.IsNullOrEmpty(p.PublicId)) // 👈 關鍵！只抓已經存在資料庫的老照片，避開剛 Add 進去的新照片
                            .Where(p => !keepPublicIds.Contains(p.PublicId))
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
            return dto;
        }
        #endregion

        #region 課程模板展示
        public async Task<List<DCourseInfo>> ThisTemp( int currentUserId) {
            // 1. 先確認這個 User 是不是教練，並拿到他的 CoachId
            var coach = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.UserId == currentUserId);
            if (coach == null) throw new InvalidOperationException("教練不匹配");

            // 2. 抓取該教練所有的模板
            var templates = await _context.ExpCourseTemplates
                .Where(t => t.CoachId == coach.Id)
                .Select(t => new DCourseInfo
                {
                    TempId = t.Id, // 記得傳 ID，前端編輯才抓得到
                    CoachId = t.CoachId,
                    Title = t.Title,
                    Description = t.Description,
                    ImageUrls = t.ExpCoursePhotos.Select(p => new DPhoto {
                        PhotoUrl = p.PhotoUrl,
                        PublicId = p.PublicId
                    }).ToList(),
                    Price = t.Price,
                    Difficulty = t.Difficulty,
                    Location = t.Location,
                    UpdatedAt = t.UpdatedAt 
                }).ToListAsync();

            return templates;
        }
        #endregion

        #region 課程選時間上架 
        public async Task<DCourseOpenSession> OpenSession(DCourseOpenSession dto, int TemplateId, int currentUserId)
        {
            //找有沒有模板
            var t = await _context.ExpCourseTemplates.FirstOrDefaultAsync(t => t.Id == TemplateId);
            if (t == null) { throw new Exception("凡人不能看的天書"); }
            //使用者和教練對得上
            var coach = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.Id == t.CoachId);
            if (coach == null || coach.UserId != currentUserId)
                throw new InvalidOperationException("教練不匹配");

            if (dto.StartDate == null) return null;

            //選時間和人數
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly day60 = today.AddDays(60);

            DateOnly date = dto.StartDate.Value;

            if (date < today || date > day60)
            
                return null;
            
            //找有沒有衝堂
            bool isConflict = await _context.ExpCourseSessions.AnyAsync(s =>
                    s.CoachId == t.CoachId &&
                    s.StartDate == date &&
                    s.TimeSlot == dto.TimeSlot);
                if (isConflict) throw new Exception("尚未習得隱分身之術 你逆");

                var newSession = new ExpCourseSession
                {
                    CourseTemplateId = TemplateId,
                    CoachId = t.CoachId,
                    StartDate = date,
                    TimeSlot = dto.TimeSlot,
                    MaxParticipants = dto.MaxParticipants,
                    CurrentParticipants = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _context.ExpCourseSessions.AddAsync(newSession);
            
            await _context.SaveChangesAsync();
            return dto;

        }
        #endregion

        #region 拿該教練所有「已上架」的課程時段
        public async Task<List<DCourseInfo>> GetAllPublishedSessions(int currentUserId)
        {
            // 1. 先確認身分並拿到 CoachId
            var coach = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.UserId == currentUserId);
            if (coach == null) throw new InvalidOperationException("教練不匹配");

            // 2. 抓取該教練所有已上架的 Session (包含模板資訊)
            var sessions = await _context.ExpCourseSessions
                .Where(s => s.CoachId == coach.Id)
                .OrderByDescending(s => s.UpdatedAt) // 最新上架的排前面
                .Select(s => new DCourseInfo
                {
                    // Session 自身的資訊 (這才是刪除/下架要用的 ID)
                    SessionId = s.Id,
                    TimeSlot = s.TimeSlot,
                    MaxParticipants = s.MaxParticipants,
                    CurrentParticipants = s.CurrentParticipants,
                    StartDate = s.StartDate,
                    CoachUserId = s.CourseTemplate.Coach.UserId,
                    // 關聯模板的資訊 (從 CourseTemplate 導航屬性抓)
                    TempId = s.CourseTemplateId,
                    Title = s.CourseTemplate.Title,
                    Price = s.CourseTemplate.Price,
                    Location = s.CourseTemplate.Location,
                    Difficulty = s.CourseTemplate.Difficulty,
                    Description = s.CourseTemplate.Description,

                    // 抓模板的第一張照片作為縮圖
                    ImageUrls = s.CourseTemplate.ExpCoursePhotos.Select(p => new DPhoto
                    {
                        PhotoUrl = p.PhotoUrl,
                        PublicId = p.PublicId
                    }).ToList(),

                    UpdatedAt = s.UpdatedAt
                }).ToListAsync();

            return sessions;
        }
        #endregion

        #region 日期找課 
        public async Task<List<DCourseInfo>> CourseByDates(string day, int coachId) {

            if (!DateOnly.TryParse(day, out var targetDate))
                throw new InvalidOperationException("沒課");

            var todayCourse = await _context.ExpCourseSessions
                .Where(d => d.CoachId == coachId && d.StartDate == DateOnly.Parse(day))
                .Select(d => new DCourseInfo
                {
                    TempId = d.Id, 
                    CoachId = d.CoachId,
                    Title = d.CourseTemplate.Title,
                    Description = d.CourseTemplate.Description,
                    ImageUrls = d.CourseTemplate.ExpCoursePhotos.Select(p => new DPhoto
                    {
                        PhotoUrl = p.PhotoUrl,
                        PublicId = p.PublicId
                    }).ToList(),
                    Price = d.CourseTemplate.Price,
                    Difficulty = d.CourseTemplate.Difficulty,
                    Location = d.CourseTemplate.Location,
                    UpdatedAt = d.CourseTemplate.UpdatedAt,
                    CoachUserId = d.CourseTemplate.Coach.UserId,

                }).ToListAsync();
            return todayCourse;

        }
        #endregion

        #region 課程時段刪除
        public async Task<string> DeleteCourseSession(int SessionId, int currentUserId)
        {
            var session = await _context.ExpCourseSessions.FirstOrDefaultAsync(s => s.Id == SessionId);
            if (session == null)
                throw new Exception("沒有對應的資料");

            var c = await _context.ExpCoaches.FirstOrDefaultAsync(c => c.UserId == currentUserId);
            if (c == null || session.CoachId != c.Id)
                throw new InvalidOperationException("教練不匹配");

            if (session.CurrentParticipants > 0)
                throw new InvalidOperationException("有學生報名了，不能耍任性");
                  

            _context.ExpCourseSessions.Remove(session);
            await _context.SaveChangesAsync();
            return "刪除成功";

        }

        #endregion

        #region 課程大眾展示介紹
        public async Task<DCourseInfo> ThisCourse(int courseId)
        {
            var courseSession = await _context.ExpCourseSessions
                                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (courseSession == null) throw new InvalidOperationException("課程不匹配");

            var result = await _context.ExpCourseSessions
                    .Where(c => c.Id == courseId)
                    .Select(c => new DCourseInfo
                    {
                        CoachId = c.CoachId,

                        SessionId = c.Id,
                        TempId = c.CourseTemplateId,
                        StartDate = c.StartDate,
                        TimeSlot = c.TimeSlot,
                        MaxParticipants = c.MaxParticipants,
                        CurrentParticipants = c.CurrentParticipants,
                        UpdatedAt = c.UpdatedAt,
                        Title = c.CourseTemplate.Title,
                        CoachUserId = c.CourseTemplate.Coach.User.Id,
                        Description = c.CourseTemplate.Description
                    }).FirstOrDefaultAsync();

            if (result == null)
                throw new InvalidOperationException("課程找不到");

            return result;
        }
        #endregion

        #region 最新開課
        public async Task<DCourseInfo> LatestCourseByCoach(int coachId)
        {
            var result = await _context.ExpCourseSessions
                .Where(c => c.CoachId == coachId)          // 篩這個教練的所有課
                .OrderByDescending(c => c.UpdatedAt)        // 最新更新的排最前面
                .Select(c => new DCourseInfo
                {
                    CoachId = c.CoachId,
                    CoachUserId = c.CourseTemplate.Coach.User.Id,
                    StartDate = c.StartDate,
                    TimeSlot = c.TimeSlot,
                    MaxParticipants = c.MaxParticipants,
                    CurrentParticipants = c.CurrentParticipants,
                    UpdatedAt = c.UpdatedAt,
                    Title = c.CourseTemplate.Title,
                    Price = c.CourseTemplate.Price,
                    Location = c.CourseTemplate.Location,
                    ImageUrls = c.CourseTemplate.ExpCoursePhotos.Select(p => new DPhoto
                    {
                        PhotoUrl = p.PhotoUrl
                    }).ToList()
                })
                .FirstOrDefaultAsync();                    

            //if (result == null)
            //    throw new InvalidOperationException("課程找不到");

            return result;
        }
        #endregion

        #region 參加過的課
        public async Task<List<DCourseOrder>> GetUserCourseHistory(int userId) {
            var history = await _context.ExpCourseOrders
                    .Where(o => o.UserId == userId && o.Status == 1)
                    .Select(o => new DCourseOrder
                    {
                        // --- 課程與時段資料 ---
                        CourseSessionId = o.CourseSessionId,
                        UserId = o.UserId,
                        Title = o.CourseSession.CourseTemplate.Title,
                        Price = o.CourseSession.CourseTemplate.Price,
                        StartDate = o.CourseSession.StartDate,
                        TimeSlot = o.CourseSession.TimeSlot,
                        Location = o.CourseSession.CourseTemplate.Location,
                        Difficulty = o.CourseSession.CourseTemplate.Difficulty,
                        CourseOrderId = o.Id,
                        // --- 教練資料 ---
                        CoachId = o.CourseSession.CoachId,
                        CoachName = o.CourseSession.CourseTemplate.Coach.Name,
                        AvatarUrl = o.CourseSession.CourseTemplate.Coach.AvatarUrl,
                        // --- 評論資料 ---
                        ReviewId = o.ExpReviews.Select(r => (int?)r.Id).FirstOrDefault(),
                        ReviewContent = o.ExpReviews
                            .Select(r => r.ReviewContent).FirstOrDefault(),
                        CreatReviewAt = o.ExpReviews
                             .Select(r => r.ReviewedAt).FirstOrDefault(),
                        UpdateReviewAt = o.ExpReviews
                            .Select(r => r.UpdateAt).FirstOrDefault(),
                        Rating = o.ExpReviews
                            .Select(r => r.Rating).FirstOrDefault(),
                        // --- 交易狀態 ---
                        ExpTransactionId = o.ExpTransactionId,
                        ReservedAt = o.ReservedAt,
                        UpdatedTransacAt = o.UpdatedAt,
                        //Status = o.Status
                    }).ToListAsync();
            foreach (var h in history)
                Console.WriteLine($"OrderId={h.CourseSessionId}, ReviewId={h.ReviewId}, Rating={h.Rating}");
            return history;

        }
        #endregion

        #region 所有開課日-月利用
        public async Task<List<string>> GetCoachCourseDatesAsync(int coachId) {
            return await _context.ExpCourseSessions
                        .Where(c => c.CoachId == coachId)
                        .Select(c => c.StartDate.ToString())
                        .Distinct()
                        .ToListAsync();
        }

        #endregion

        #region 所有開的課
        public async Task<List<DCourseInfo>> GetCoursesByDateAsync(int coachId, DateOnly date)
        {
            return await _context.ExpCourseSessions
                .Where(c => c.CoachId == coachId && c.StartDate == date)
                .Select(c => new DCourseInfo
                {
                    SessionId = c.Id,
                    TempId = c.CourseTemplateId,
                    CoachId = c.CoachId,
                    StartDate = c.StartDate,
                    TimeSlot = c.TimeSlot,
                    MaxParticipants = c.MaxParticipants,
                    CurrentParticipants = c.CurrentParticipants,
                    UpdatedAt = c.UpdatedAt,
                    Title = c.CourseTemplate.Title,
                    Price = c.CourseTemplate.Price,
                    Location = c.CourseTemplate.Location,
                    Description = c.CourseTemplate.Description,
                    ImageUrls = c.CourseTemplate.ExpCoursePhotos.Select(p => new DPhoto
                    {
                        PhotoUrl = p.PhotoUrl,
                        PublicId = p.PublicId
                    }).ToList(),
                }).ToListAsync();
        }
        #endregion
        #endregion

        #region ~~評論~~
        #region 新增評論
        public async Task<string> CreateReview(DReview dto, int userId, int courseOId)
        {
            try
            {
                var order = await _context.ExpCourseOrders
                     .Where(o => o.Id == courseOId && o.UserId == userId)
                     .Select(o => new { CoachId = o.CourseSession.CoachId })
                     .FirstOrDefaultAsync();

                if (order == null) return "沒有課程可以評論";

                var newReview = new ExpReview
                {
                    UserId = userId,
                    CoachId = order.CoachId,
                    CourseOrderId = courseOId,
                    Rating = dto.Rating,
                    ReviewContent = dto.ReviewContent,
                    ReviewedAt = DateTime.Now,
                    IsHidden = false
                };

                await _context.ExpReviews.AddAsync(newReview);
                var result = await _context.SaveChangesAsync();

                // 教練提醒：檢查 SaveChanges 回傳的數字，應該要大於 0 才算成功
                if (result > 0)
                {
                    return "~感謝大大撥冗評論~";
                }
                else
                {
                    return "寫入失敗，但沒噴報錯";
                }
            }
            catch (Exception ex)
            {
                // 這裡會告訴你到底是哪個欄位出事了！
                return $"系統錯誤：{ex.InnerException?.Message ?? ex.Message}";
            }
        }
        #endregion

        #region 編輯評論
        public async Task<DReview> EditReview(DReview dto, int userId, int courseId, int reviewId)
        {
            var r = await _context.ExpReviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if (r == null) throw new InvalidOperationException("評論不匹配");

            //進行修改：直接把 DTO 的資料倒進去 Entity(原本的那筆)
            r.ReviewContent = dto.ReviewContent;
            r.Rating = dto.Rating;
            r.UpdateAt = DateTime.Now;
           


            await _context.SaveChangesAsync();
            return dto;
        }
        #endregion

        #region 刪除評論
        public async Task<string> DeleteReview(int userId, int reviewId)
        {
            var review = await _context.ExpReviews.FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);
            if (review == null)
                throw new Exception("沒有對應的資料");
            _context.ExpReviews.Remove(review);
            await _context.SaveChangesAsync();
            return  "成功刪除評論" ;
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

        #region 拿最新三筆
        public async Task<IEnumerable<DReview>> ThreeReviewsByCoach(int coachId)
        {
            var result = await _context.ExpReviews
                .Where(c => c.CoachId == coachId)         
                .OrderByDescending(c => c.Id)   
                .Take(3)
                .Select(c => new DReview
                {
                    CoachId = c.CoachId,
                    UserName = c.User.UserName,
                    ReviewId = c.Id,
                    Rating = c.Rating,
                    ReviewContent = c.ReviewContent,
                    CourseOrderId = c.CourseOrderId,
                    ReviewedAt = c.ReviewedAt,
                    UpdateAt = c.UpdateAt
                })
                .ToListAsync();                     

            if (result == null)
                throw new InvalidOperationException("課程不匹配");
            return result;
        }
        #endregion
        #endregion
        #region 專業運動種類
        public async Task<List<DSpeciallity>> Sports() { 
            var result = await _context.ExpSpecialities
                .Select(s => new DSpeciallity
                {
                    Id = s.Id,
                    SportsName = s.SportsName
                }).ToListAsync();
            return result;
        }
        #endregion


        #region 交易流程
        #region 新增預約課程
        public async Task<object> SessionReserve(DCourseOrder dto, int userId)
        {
            //找這堂課，順便拿教練ID
            var session = await _context.ExpCourseSessions
                          .Include(s => s.CourseTemplate)
                          .Include(s => s.CourseTemplate.Coach)
                          .FirstOrDefaultAsync(r => r.Id == dto.CourseSessionId);
            if (session == null) throw new Exception("沒有對應的資料");

            //核對課堂名額
            if (session.CurrentParticipants >= session.MaxParticipants)
                throw new Exception("額滿");

            //建立一筆 ExpTransaction，取得 TransactionId
            decimal? coursePrice = session.CourseTemplate.Price;
            var transac = new ExpTransaction
            {
                SenderUserId = userId,
                //ReceiveUserId = null,
                ReceiveUserId = session.CourseTemplate.Coach.UserId,
                Amount = session.CourseTemplate?.Price ?? 0,            // 假設金額，之後可從 Template 抓
                Status = 0,                      // 0: 已建立/待付款
                TypeId = 3,
                CreatedAt = DateTime.Now
            };
            Console.WriteLine($"session.CoachId = {session?.CoachId}");
            Console.WriteLine($"CourseTemplate = {session?.CourseTemplate?.Id}");
            Console.WriteLine($"Coach = {session?.CourseTemplate?.Coach?.Id}");
            Console.WriteLine($"Coach.UserId = {session?.CourseTemplate?.Coach?.UserId}");
            Console.WriteLine($"SenderUserId = {userId}");
            Console.WriteLine($" ReceiveUserId={session.CoachId}");
            await _context.ExpTransactions.AddAsync(transac);
            await _context.SaveChangesAsync();

            //建立預約實體 (Entity)
            var reserve = new ExpCourseOrder
            {
                UserId = userId,
                CourseSessionId = session.Id,
                ReservedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ExpTransactionId = transac.Id,
                Status = 0
            };
            //課程報名人+1
            //session.CurrentParticipants += 1;

            await _context.ExpCourseOrders.AddAsync(reserve);
            await _context.SaveChangesAsync();
            return transac.Id;
        }
        #endregion
        #region 歷史交易紀錄 
        public async Task<List<DTransac>> TransacList(int userId)
        {
            using (var db = new SalterDbContext())
            {
                var history = await db.ExpTransactions
                            .Where(h => h.SenderUserId == userId)
                            .Select(h => new DTransac
                            {
                                TransactionId = h.Id,
                                CourseName = h.ExpCourseOrders.FirstOrDefault().CourseSession.CourseTemplate.Title,
                                CoaId = h.ExpCourseOrders.FirstOrDefault().CourseSession.CoachId,
                                Amount = h.Amount,
                                Status = h.Status == 0 ? "等待付款" :
                                        h.Status == 1 ? "交易成功" : "已取消",
                                OrderDate = h.CreatedAt
                            })
                            .OrderByDescending(h => h.OrderDate)
                            .ToListAsync();
                return history;
            }

        }
        #endregion

        #endregion
        #region 呼叫TripLocation
        private async Task<int> EnsureLocationExistsAsync(TripLocationRequestDto dto)
        {
            var normalizedCity = dto.CityName?.Replace("臺", "台") ?? "";
            var normalizedDistrict = dto.DistrictName?.Replace("臺", "台") ?? "";

            // 1. 處理縣市
            var city = await _tr.GetCityByNameAsync(normalizedCity)
                       ?? await _tr.CreateCityAsync(new TripCity { Name = normalizedCity });

            // 2. 處理區域
            var district = await _tr.GetDistrictByNameAsync(normalizedDistrict, city.Id)
                           ?? await _tr.CreateDistrictAsync(new TripDistrict { Name = normalizedDistrict, CityId = city.Id });

            // 3. 處理地點
            var location = await _tr.GetLocationByGooglePlaceIdAsync(dto.GooglePlaceId);
            if (location == null)
            {
                location = new TripLocation
                {
                    GooglePlaceId = dto.GooglePlaceId,
                    Name = dto.LocationName,
                    AddressText = dto.AddressText ?? "",
                    Lat = dto.Lat ?? 0,
                    Lng = dto.Lng ?? 0,
                    CityId = city.Id,
                    DistrictId = district.Id
                };
                await _tr.CreateTripLocationAsync(location);
            }
            return location.Id; // 回傳 ID 供後續關聯使用
        }
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
