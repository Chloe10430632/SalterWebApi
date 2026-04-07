using TripServiceHelper.Models.DTOs;

namespace TripServiceHelper.IService;

public interface ITripService
{
    #region 行程
    Task<TripListResultDto> GetTripListAsync(TripQueryDto query, int? userId = null);
    Task<TripDetailDto?> GetTripDetailAsync(int tripId, int? userId = null);
    Task<ServiceResult<int>> CreateTripAsync(TripRequestDto dto, int organizerUserId);
    Task<ServiceResult> UpdateTripAsync(int tripId, TripRequestDto dto, int userId);
    Task<ServiceResult> DeleteTripAsync(int tripId, int userId);

    Task<List<TripSummaryDto>> GetMyTripsAsync(int userId, string? role = null);
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

    #region 收藏資料夾
    Task<List<TripFavoriteFolderDto>> GetFoldersAsync(int userId);
    Task<ServiceResult<TripFavoriteFolderDto>> CreateFolderAsync(TripFavoriteFolderRequestDto dto, int userId);
    Task<ServiceResult> UpdateFolderAsync(int folderId, TripFavoriteFolderRequestDto dto, int userId);
    Task<ServiceResult> DeleteFolderAsync(int folderId, int userId);
    Task<ServiceResult> MoveFavoriteToFolderAsync(int tripId, int userId, int? folderId);
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
    Task<ServiceResult<int>> CreateLocationAsync(int tripId, TripLocationRequestDto dto, int userId);
    Task<ServiceResult> UpdateLocationSortAsync(int tripId, TripLocationSortDto dto, int userId);
    Task<List<TripLocationSearchDto>> GetAllLocationsAsync(string? keyword);

    #endregion

    #region 城市
    Task<List<TripCityDto>> GetCitiesAsync();
    Task<List<TripDistrictDto>> GetDistrictsAsync(int cityId);
    
    #endregion
}