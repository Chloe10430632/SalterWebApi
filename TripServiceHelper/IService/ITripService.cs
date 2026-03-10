using TripServiceHelper.Models.DTOs;

namespace TripServiceHelper.IService;

public interface ITripService
{
    // 行程
    Task<TripListResultDto> GetTripListAsync(TripQueryDto query);
    Task<TripDetailDto?> GetTripDetailAsync(int tripId);
    Task<ServiceResult> CreateTripAsync(TripRequestDto dto, int organizerUserId);
    Task<ServiceResult> UpdateTripAsync(int tripId, TripRequestDto dto);
    Task<ServiceResult> DeleteTripAsync(int tripId);

    // 成員
    Task<ServiceResult> JoinTripAsync(int tripId, int userId);
    Task<ServiceResult> LeaveTripAsync(int tripId, int userId);

    // 收藏
    Task<List<TripSummaryDto>> GetFavoritesAsync(int userId);
    Task<ServiceResult> AddFavoriteAsync(int tripId, int userId);
    Task<ServiceResult> RemoveFavoriteAsync(int tripId, int userId);

    // 公告
    Task<List<TripAnnouncementDto>> GetAnnouncementsAsync(int tripId);
    Task<ServiceResult> CreateAnnouncementAsync(int tripId, TripAnnouncementRequestDto dto, int userId);
    Task<ServiceResult> UpdateAnnouncementAsync(int announcementId, TripAnnouncementRequestDto dto);
    Task<ServiceResult> DeleteAnnouncementAsync(int announcementId);
    Task<ServiceResult> TogglePinAsync(int announcementId);

    // 裝備
    Task<List<TripGearItemDto>> GetGearItemsAsync(int tripId);
    Task<ServiceResult> CreateGearItemAsync(int tripId, TripGearItemRequestDto dto, int userId);
    Task<ServiceResult> UpdateGearItemAsync(int gearItemId, TripGearItemRequestDto dto);
    Task<ServiceResult> DeleteGearItemAsync(int gearItemId);
    Task<ServiceResult> ToggleGearCheckAsync(int gearItemId, int userId);

    // 地點
    Task<List<TripLocationDto>> GetLocationsAsync(int tripId);
    Task<ServiceResult> CreateLocationAsync(int tripId, TripLocationRequestDto dto);
    Task<ServiceResult> UpdateLocationAsync(int locationId, TripLocationRequestDto dto);
    Task<ServiceResult> DeleteLocationAsync(int locationId);

    // 提醒
    Task<List<TripReminderDto>> GetRemindersAsync(int tripId, int userId);
    Task<ServiceResult> CreateReminderAsync(int tripId, TripReminderRequestDto dto, int userId);
    Task<ServiceResult> UpdateReminderAsync(int reminderId, TripReminderRequestDto dto);
    Task<ServiceResult> ToggleReminderAsync(int reminderId);

    // 城市
    Task<List<TripCityDto>> GetCitiesAsync();
    Task<List<TripDistrictDto>> GetDistrictsAsync(int cityId);
}