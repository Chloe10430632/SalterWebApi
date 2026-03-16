using SalterEFModels.EFModels;
using TripRepositoryHelper.IRepository;
using TripServiceHelper.Cloudinary;
using TripServiceHelper.IService;
using TripServiceHelper.Models.DTOs;

namespace TripServiceHelper.Service;

public class TripService : ITripService
{
    private readonly ITripRepository _repo;
    private readonly ICloudinaryTripService _cloudinary;

    public TripService(ITripRepository repo, ICloudinaryTripService cloudinary)
    {
        _repo = repo;
        _cloudinary = cloudinary;
    }

    #region 行程

    public async Task<TripListResultDto> GetTripListAsync(TripQueryDto query)
    {
        var (trips, totalCount) = await _repo.GetTripsAsync(
            query.Keyword, query.TripType, query.Status, query.CityId,
            query.StartFrom, query.StartTo,
            query.MinCapacity, query.MaxCapacity,
            query.SortBy, query.Page, query.PageSize);

        return new TripListResultDto
        {
            Trips = trips.Select(ToSummaryDto).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<TripDetailDto?> GetTripDetailAsync(int tripId)
    {
        var trip = await _repo.GetTripByIdAsync(tripId);
        if (trip == null) return null;

        return new TripDetailDto
        {
            Id = trip.Id,
            Title = trip.Title,
            TripType = trip.TripType,
            Description = trip.Description,
            StartAt = trip.StartAt,
            EndAt = trip.EndAt,
            Capacity = trip.Capacity,
            Status = trip.Status,
            CoverImageUrl = trip.CoverImageUrl,
            OrganizerUserId = trip.OrganizerUserId,
            OrganizerName = trip.OrganizerUser?.UserName ?? "未知",
            OrganizerEmail = trip.OrganizerUser?.Email,
            LockedAt = trip.LockedAt,
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt,
            MemberCount = trip.TripMembers?.Count ?? 0,
            FavoriteCount = trip.TripFavorites?.Count ?? 0,
            AnnouncementCount = trip.TripAnnouncements?.Count ?? 0,
            GearItemCount = trip.TripGearItems?.Count ?? 0,
            Members = trip.TripMembers?.Select(m => new TripMemberDto
            {
                UserId = m.UserId,
                UserName = m.User?.UserName ?? "未知",
                Email = m.User?.Email,
                Role = m.Role,
                JoinedAt = m.JoinedAt
            }).ToList() ?? new(),
            Locations = trip.TripTripLocations?
                .OrderBy(ttl => ttl.SortOrder)
                .Select(ttl => new TripLocationDto
                {
                    Id = ttl.Id,
                    LocationName = ttl.Location?.Name ?? "",
                    CityName = ttl.Location?.District?.City?.Name ?? "",
                    DistrictName = ttl.Location?.District?.Name ?? "",
                    LocationRole = ttl.LocationRole,
                    Note = ttl.Note,
                    SortOrder = ttl.SortOrder
                }).ToList() ?? new()
        };
    }

    public async Task<ServiceResult> CreateTripAsync(TripRequestDto dto, int organizerUserId)
    {
        // 日期驗證
        if (dto.StartAt < DateTime.Now)
            return ServiceResult.Fail("開始日期不能早於今天");

        if (dto.EndAt.HasValue && dto.EndAt.Value <= dto.StartAt)
            return ServiceResult.Fail("結束日期必須晚於開始日期");


        try
        {
            var trip = new TripTrip
            {
                Title = dto.Title,
                TripType = dto.TripType,
                Description = dto.Description,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Capacity = dto.Capacity,
                Status = "active",
                OrganizerUserId = organizerUserId,
                CoverImageUrl = dto.CoverImageUrl,
                CoverImagePublicId = dto.CoverImagePublicId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repo.CreateTripAsync(trip);
            await _repo.AddMemberAsync(trip.Id, organizerUserId, "organizer");

            return ServiceResult.Success("行程建立成功");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"建立失敗：{ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<ServiceResult> UpdateTripAsync(int tripId, TripRequestDto dto, int userId)
    {
        var isOrganizer = await _repo.IsOrganizerAsync(tripId, userId);
        var isMember = await _repo.IsMemberAsync(tripId, userId);
        if (!isOrganizer && !isMember)
            return ServiceResult.Fail("只有行程成員可以編輯行程");

        var trip = await _repo.GetTripByIdAsync(tripId);
        if (trip == null) return ServiceResult.Fail("找不到行程");

        // 如果有新圖片 且 跟舊圖片不一樣  先刪除舊圖片
        if (!string.IsNullOrEmpty(trip.CoverImagePublicId) &&
            trip.CoverImagePublicId != dto.CoverImagePublicId)
        {
            await _cloudinary.DeleteImageAsync(trip.CoverImagePublicId);
        }

        trip.Title = dto.Title;
        trip.TripType = dto.TripType;
        trip.Description = dto.Description;
        trip.StartAt = dto.StartAt;
        trip.EndAt = dto.EndAt;
        trip.Capacity = dto.Capacity;
        trip.CoverImageUrl = dto.CoverImageUrl;
        trip.CoverImagePublicId = dto.CoverImagePublicId;
        trip.UpdatedAt = DateTime.Now;

        await _repo.UpdateTripAsync(trip);
        return ServiceResult.Success("行程更新成功");
    }

    public async Task<ServiceResult> DeleteTripAsync(int tripId, int userId)
    {
        try
        {
            if (!await _repo.IsOrganizerAsync(tripId, userId))
                return ServiceResult.Fail("只有主辦人可以刪除行程");

            var trip = await _repo.GetTripByIdAsync(tripId);
            if (trip == null) return ServiceResult.Fail("找不到行程");

            // 如果有封面圖，先刪除 Cloudinary 的圖片
            if (!string.IsNullOrEmpty(trip.CoverImagePublicId))
                await _cloudinary.DeleteImageAsync(trip.CoverImagePublicId);

            var result = await _repo.DeleteTripAsync(tripId);
            return result ? ServiceResult.Success("行程刪除成功") : ServiceResult.Fail("刪除失敗");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"刪除失敗：{ex.InnerException?.Message ?? ex.Message}");
        }
    }

    #endregion

    #region 成員

    public async Task<ServiceResult> JoinTripAsync(int tripId, int userId)
    {
        var trip = await _repo.GetTripByIdAsync(tripId);
        if (trip == null) return ServiceResult.Fail("找不到行程");

        if (trip.Status != "active")
            return ServiceResult.Fail("此行程已無法加入");

        if (trip.TripMembers?.Count >= trip.Capacity)
            return ServiceResult.Fail("行程人數已滿");

        var result = await _repo.AddMemberAsync(tripId, userId, "member");
        return result ? ServiceResult.Success("加入成功") : ServiceResult.Fail("已經是成員");
    }

    public async Task<ServiceResult> LeaveTripAsync(int tripId, int userId)
    {
        if (await _repo.IsOrganizerAsync(tripId, userId))
            return ServiceResult.Fail("主辦人不能退出行程");

        var result = await _repo.RemoveMemberAsync(tripId, userId);
        return result ? ServiceResult.Success("退出成功") : ServiceResult.Fail("你不是此行程的成員");
    }

    #endregion

    #region 收藏

    public async Task<List<TripSummaryDto>> GetFavoritesAsync(int userId)
    {
        var favorites = await _repo.GetFavoritesAsync(userId);
        return favorites
            .Where(f => f.Trip != null)
            .Select(f => ToSummaryDto(f.Trip!))
            .ToList();
    }

    public async Task<ServiceResult> AddFavoriteAsync(int tripId, int userId)
    {
        var result = await _repo.AddFavoriteAsync(tripId, userId);
        return result ? ServiceResult.Success("收藏成功") : ServiceResult.Fail("已收藏過");
    }

    public async Task<ServiceResult> RemoveFavoriteAsync(int tripId, int userId)
    {
        var result = await _repo.RemoveFavoriteAsync(tripId, userId);
        return result ? ServiceResult.Success("取消收藏成功") : ServiceResult.Fail("尚未收藏");
    }

    #endregion

    #region 公告
    public async Task<ServiceResult<List<TripAnnouncementDto>>> GetAnnouncementsAsync(int tripId, int userId)
    {
        var isOrganizer = await _repo.IsOrganizerAsync(tripId, userId);
        var isMember = await _repo.IsMemberAsync(tripId, userId);

        if (!isOrganizer && !isMember)
            return ServiceResult<List<TripAnnouncementDto>>.Fail("請先加入行程才能查看公告");

        var list = await _repo.GetAnnouncementsAsync(tripId);
        var result = list.Select(a => new TripAnnouncementDto
        {
            Id = a.Id,
            Title = a.Title,
            Content = a.Content,
            IsPinned = a.IsPinned,
            CreatedByUserId = a.CreatedByUserId,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();

        return ServiceResult<List<TripAnnouncementDto>>.Success(result);
    }

    public async Task<ServiceResult> CreateAnnouncementAsync(int tripId, TripAnnouncementRequestDto dto, int userId)
    {
        var entity = new TripAnnouncement
        {
            TripId = tripId,
            Title = dto.Title,
            Content = dto.Content,
            IsPinned = false,
            CreatedByUserId = userId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repo.CreateAnnouncementAsync(entity);
        return ServiceResult.Success("公告新增成功");
    }

    public async Task<ServiceResult> UpdateAnnouncementAsync(int announcementId, TripAnnouncementRequestDto dto, int userId)
    {
        var entity = await _repo.GetAnnouncementByIdAsync(announcementId);
        if (entity == null) return ServiceResult.Fail("找不到公告");

        if (!await _repo.IsOrganizerAsync(entity.TripId, userId))
            return ServiceResult.Fail("只有主辦人可以編輯公告");

        entity.Title = dto.Title;
        entity.Content = dto.Content;
        entity.UpdatedAt = DateTime.Now;
        await _repo.UpdateAnnouncementAsync(entity);
        return ServiceResult.Success("公告更新成功");
    }

    public async Task<ServiceResult> DeleteAnnouncementAsync(int announcementId, int userId)
    {
        var entity = await _repo.GetAnnouncementByIdAsync(announcementId);
        if (entity == null) return ServiceResult.Fail("找不到公告");

        if (!await _repo.IsOrganizerAsync(entity.TripId, userId))
            return ServiceResult.Fail("只有主辦人可以刪除公告");

        await _repo.DeleteAnnouncementAsync(announcementId);
        return ServiceResult.Success("公告刪除成功");
    }

    public async Task<ServiceResult> TogglePinAsync(int announcementId, int userId)
    {
        var entity = await _repo.GetAnnouncementByIdAsync(announcementId);
        if (entity == null) return ServiceResult.Fail("找不到公告");

        if (!await _repo.IsOrganizerAsync(entity.TripId, userId))
            return ServiceResult.Fail("只有主辦人可以置頂公告");

        await _repo.TogglePinAnnouncementAsync(announcementId);
        return ServiceResult.Success("置頂狀態已更新");
    }

    #endregion

    #region 裝備

    public async Task<ServiceResult<List<TripGearItemDto>>> GetGearItemsAsync(int tripId, int userId)
    {
        var isOrganizer = await _repo.IsOrganizerAsync(tripId, userId);
        var isMember = await _repo.IsMemberAsync(tripId, userId);

        if (!isOrganizer && !isMember)
            return ServiceResult<List<TripGearItemDto>>.Fail("請先加入行程才能查看裝備清單");

        var list = await _repo.GetGearItemsAsync(tripId);
        var result = list.Select(g => new TripGearItemDto
        {
            Id = g.Id,
            ItemName = g.ItemName,
            IsRequired = g.IsRequired,
            IsCheckedByMe = g.TripGearChecks?.Any(c => c.UserId == userId && c.IsChecked) ?? false,
            CheckedCount = g.TripGearChecks?.Count(c => c.IsChecked) ?? 0
        }).ToList();

        return ServiceResult<List<TripGearItemDto>>.Success(result);
    }

    public async Task<ServiceResult> CreateGearItemAsync(int tripId, TripGearItemRequestDto dto, int userId)
    {
        var entity = new TripGearItem
        {
            TripId = tripId,
            ItemName = dto.ItemName,
            IsRequired = dto.IsRequired,
            CreatedByUserId = userId
        };
        await _repo.CreateGearItemAsync(entity);
        return ServiceResult.Success("裝備新增成功");
    }

    public async Task<ServiceResult> UpdateGearItemAsync(int gearItemId, TripGearItemRequestDto dto, int userId)
    {
        var entity = await _repo.GetGearItemByIdAsync(gearItemId);
        if (entity == null) return ServiceResult.Fail("找不到裝備");

        var isOrganizer = await _repo.IsOrganizerAsync(entity.TripId, userId);
        var isMember = await _repo.IsMemberAsync(entity.TripId, userId);
        if (!isOrganizer && !isMember)
            return ServiceResult.Fail("只有行程成員可以編輯裝備");

        entity.ItemName = dto.ItemName;
        entity.IsRequired = dto.IsRequired;
        await _repo.UpdateGearItemAsync(entity);
        return ServiceResult.Success("裝備更新成功");
    }

    public async Task<ServiceResult> DeleteGearItemAsync(int gearItemId, int userId)
    {
        var entity = await _repo.GetGearItemByIdAsync(gearItemId);
        if (entity == null) return ServiceResult.Fail("找不到裝備");

        if (!await _repo.IsOrganizerAsync(entity.TripId, userId))
            return ServiceResult.Fail("只有主辦人可以刪除裝備");

        await _repo.DeleteGearItemAsync(gearItemId);
        return ServiceResult.Success("裝備刪除成功");
    }

    public async Task<ServiceResult> ToggleGearCheckAsync(int gearItemId, int userId)
    {
        var result = await _repo.ToggleGearCheckAsync(gearItemId, userId);
        return result ? ServiceResult.Success("勾選狀態已更新") : ServiceResult.Fail("操作失敗");
    }

    #endregion

    #region 地點

    public async Task<ServiceResult<List<TripLocationDto>>> GetLocationsAsync(int tripId, int userId)
    {
        var isOrganizer = await _repo.IsOrganizerAsync(tripId, userId);
        var isMember = await _repo.IsMemberAsync(tripId, userId);

        if (!isOrganizer && !isMember)
            return ServiceResult<List<TripLocationDto>>.Fail("請先加入行程才能查看地點");

        var list = await _repo.GetLocationsAsync(tripId);
        var result = list.Select(ttl => new TripLocationDto
        {
            Id = ttl.Id,
            LocationName = ttl.Location?.Name ?? "",
            CityName = ttl.Location?.District?.City?.Name ?? "",
            DistrictName = ttl.Location?.District?.Name ?? "",
            LocationRole = ttl.LocationRole,
            Note = ttl.Note,
            SortOrder = ttl.SortOrder
        }).ToList();

        return ServiceResult<List<TripLocationDto>>.Success(result);
    }

    public async Task<ServiceResult> CreateLocationAsync(int tripId, TripLocationRequestDto dto, int userId)
    {
        var isOrganizer = await _repo.IsOrganizerAsync(tripId, userId);
        var isMember = await _repo.IsMemberAsync(tripId, userId);
        if (!isOrganizer && !isMember)
            return ServiceResult.Fail("只有行程成員可以新增地點");

        var entity = new TripTripLocation
        {
            TripId = tripId,
            LocationId = dto.LocationId,
            LocationRole = dto.LocationRole,
            Note = dto.Note,
            SortOrder = dto.SortOrder,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await _repo.CreateLocationAsync(entity);
        return ServiceResult.Success("地點新增成功");
    }

    public async Task<ServiceResult> UpdateLocationAsync(int locationId, TripLocationRequestDto dto, int userId)
    {
        var entity = await _repo.GetLocationByIdAsync(locationId);
        if (entity == null) return ServiceResult.Fail("找不到地點");

        var isOrganizer = await _repo.IsOrganizerAsync(entity.TripId, userId);
        var isMember = await _repo.IsMemberAsync(entity.TripId, userId);
        if (!isOrganizer && !isMember)
            return ServiceResult.Fail("只有行程成員可以編輯地點");

        entity.LocationRole = dto.LocationRole;
        entity.Note = dto.Note;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedAt = DateTime.Now;
        await _repo.UpdateLocationAsync(entity);
        return ServiceResult.Success("地點更新成功");
    }

    public async Task<ServiceResult> DeleteLocationAsync(int locationId, int userId)
    {
        var entity = await _repo.GetLocationByIdAsync(locationId);
        if (entity == null) return ServiceResult.Fail("找不到地點");

        if (!await _repo.IsOrganizerAsync(entity.TripId, userId))
            return ServiceResult.Fail("只有主辦人可以刪除地點");

        await _repo.DeleteLocationAsync(locationId);
        return ServiceResult.Success("地點刪除成功");
    }

    #endregion

    #region 提醒

    public async Task<ServiceResult<List<TripReminderDto>>> GetRemindersAsync(int tripId, int userId)
    {
        var isOrganizer = await _repo.IsOrganizerAsync(tripId, userId);
        var isMember = await _repo.IsMemberAsync(tripId, userId);

        if (!isOrganizer && !isMember)
            return ServiceResult<List<TripReminderDto>>.Fail("請先加入行程才能查看提醒");

        var list = await _repo.GetRemindersAsync(tripId, userId);
        var result = list.Select(r => new TripReminderDto
        {
            Id = r.Id,
            RemindOffsetMinutes = r.RemindOffsetMinutes,
            IsEnabled = r.IsEnabled,
            LastSentAt = r.LastSentAt
        }).ToList();

        return ServiceResult<List<TripReminderDto>>.Success(result);
    }

    public async Task<ServiceResult> CreateReminderAsync(int tripId, TripReminderRequestDto dto, int userId)
    {
        var entity = new TripReminder
        {
            TripId = tripId,
            UserId = userId,
            RemindOffsetMinutes = dto.RemindOffsetMinutes,
            IsEnabled = dto.IsEnabled
        };
        await _repo.CreateReminderAsync(entity);
        return ServiceResult.Success("提醒新增成功");
    }

    public async Task<ServiceResult> UpdateReminderAsync(int reminderId, TripReminderRequestDto dto, int userId)
    {
        var entity = await _repo.GetReminderByIdAsync(reminderId);
        if (entity == null) return ServiceResult.Fail("找不到提醒");

        if (entity.UserId != userId)
            return ServiceResult.Fail("只能修改自己的提醒");

        entity.RemindOffsetMinutes = dto.RemindOffsetMinutes;
        entity.IsEnabled = dto.IsEnabled;
        await _repo.UpdateReminderAsync(entity);
        return ServiceResult.Success("提醒更新成功");
    }

    public async Task<ServiceResult> ToggleReminderAsync(int reminderId)
    {
        var result = await _repo.ToggleReminderAsync(reminderId);
        return result ? ServiceResult.Success("提醒狀態已更新") : ServiceResult.Fail("找不到提醒");
    }

    #endregion

    #region 城市

    public async Task<List<TripCityDto>> GetCitiesAsync()
    {
        var list = await _repo.GetCitiesAsync();
        return list.Select(c => new TripCityDto { Id = c.Id, Name = c.Name }).ToList();
    }

    public async Task<List<TripDistrictDto>> GetDistrictsAsync(int cityId)
    {
        var list = await _repo.GetDistrictsAsync(cityId);
        return list.Select(d => new TripDistrictDto { Id = d.Id, Name = d.Name }).ToList();
    }

    #endregion

    #region Manual Mappings方法

    private static TripSummaryDto ToSummaryDto(TripTrip t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        TripType = t.TripType,
        Description = t.Description,
        StartAt = t.StartAt,
        EndAt = t.EndAt,
        Capacity = t.Capacity,
        MemberCount = t.TripMembers?.Count ?? 0,
        Status = t.Status,
        CoverImageUrl = t.CoverImageUrl,
        OrganizerName = t.OrganizerUser?.UserName ?? "未知",
        FavoriteCount = t.TripFavorites?.Count ?? 0,
        CreatedAt = t.CreatedAt
    };

    #endregion
}