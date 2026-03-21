using TripServiceHelper.Models.DTOs;

namespace TripServiceHelper.IService;

public interface ITripService
{
    #region 行程
    Task<TripListResultDto> GetTripListAsync(TripQueryDto query);
    Task<TripDetailDto?> GetTripDetailAsync(int tripId);
    Task<ServiceResult> CreateTripAsync(TripRequestDto dto, int organizerUserId);
    Task<ServiceResult> UpdateTripAsync(int tripId, TripRequestDto dto, int userId);
    Task<ServiceResult> DeleteTripAsync(int tripId, int userId);
    #endregion

    #region 成員
    Task<ServiceResult> JoinTripAsync(int tripId, int userId);
    Task<ServiceResult> LeaveTripAsync(int tripId, int userId);
    #endregion

    #region 收藏
    Task<List<TripSummaryDto>> GetFavoritesAsync(int userId);
    Task<ServiceResult> AddFavoriteAsync(int tripId, int userId);
    Task<ServiceResult> RemoveFavoriteAsync(int tripId, int userId);
    #endregion

   #region 公告
    Task<ServiceResult<List<TripAnnouncementDto>>> GetAnnouncementsAsync(int tripId, int userId);
    Task<ServiceResult> CreateAnnouncementAsync(int tripId, TripAnnouncementRequestDto dto, int userId);
    Task<ServiceResult> UpdateAnnouncementAsync(int announcementId, TripAnnouncementUpdateDto dto, int userId);
    Task<ServiceResult> DeleteAnnouncementAsync(int announcementId, int userId);
    Task<ServiceResult> TogglePinAsync(int announcementId, int userId);
    #endregion

    #region 裝備
    Task<ServiceResult<List<TripGearItemDto>>> GetGearItemsAsync(int tripId, int userId);
    Task<ServiceResult> CreateGearItemAsync(int tripId, TripGearItemRequestDto dto, int userId);
    Task<ServiceResult> UpdateGearItemAsync(int gearItemId, TripGearItemRequestDto dto, int userId);
    Task<ServiceResult> DeleteGearItemAsync(int gearItemId, int userId);
    Task<ServiceResult> ToggleGearCheckAsync(int gearItemId, int userId);
    #endregion

    #region 地點
    Task<ServiceResult<List<TripLocationDto>>> GetLocationsAsync(int tripId, int userId);
    Task<ServiceResult> UpdateLocationAsync(int locationId, TripLocationUpdateDto dto, int userId);
    Task<ServiceResult> DeleteLocationAsync(int locationId, int userId);
    Task<ServiceResult> CreateLocationAsync(int tripId, TripLocationRequestDto dto, int userId);
    Task<List<TripLocationSearchDto>> GetAllLocationsAsync(string? keyword);
    #endregion

    #region 提醒
    Task<ServiceResult<List<TripReminderDto>>> GetRemindersAsync(int tripId, int userId);
    Task<ServiceResult> CreateReminderAsync(int tripId, TripReminderRequestDto dto, int userId);
    Task<ServiceResult> UpdateReminderAsync(int reminderId, TripReminderRequestDto dto, int userId);
    Task<ServiceResult> ToggleReminderAsync(int reminderId);
    #endregion

    #region 城市
    Task<List<TripCityDto>> GetCitiesAsync();
    Task<List<TripDistrictDto>> GetDistrictsAsync(int cityId);
    
    #endregion
}