using SalterEFModels.EFModels;

namespace TripRepositoryHelper.IRepository;

public interface ITripRepository
{
    #region 行程
    Task<(List<TripTrip> trips, int totalCount)> GetTripsAsync(
        string? keyword, string? tripType, string? status, int? cityId,
        DateTime? startFrom, DateTime? startTo,
        int? minCapacity, int? maxCapacity,
        string? sortBy, int page, int pageSize);
    Task<TripTrip?> GetTripByIdAsync(int tripId);
    Task<TripTrip> CreateTripAsync(TripTrip trip);
    Task<TripTrip?> UpdateTripAsync(TripTrip trip);
    Task<bool> DeleteTripAsync(int tripId);
    #endregion

    #region 成員
    // 成員
    Task<bool> AddMemberAsync(int tripId, int userId, string role);
    Task<bool> RemoveMemberAsync(int tripId, int userId);
    #endregion

    #region 收藏
    // 收藏
    Task<List<TripFavorite>> GetFavoritesAsync(int userId);
    Task<bool> AddFavoriteAsync(int tripId, int userId);
    Task<bool> RemoveFavoriteAsync(int tripId, int userId);
    #endregion

    #region 公告
    // 公告
    Task<List<TripAnnouncement>> GetAnnouncementsAsync(int tripId);
    Task<TripAnnouncement> CreateAnnouncementAsync(TripAnnouncement entity);
    Task<TripAnnouncement?> UpdateAnnouncementAsync(TripAnnouncement entity);
    Task<TripAnnouncement?> GetAnnouncementByIdAsync(int announcementId);
    Task<bool> DeleteAnnouncementAsync(int announcementId);
    Task<bool> TogglePinAnnouncementAsync(int announcementId);
    #endregion

    #region 裝備清單
    // 裝備清單
    Task<List<TripGearItem>> GetGearItemsAsync(int tripId);
    Task<TripGearItem> CreateGearItemAsync(TripGearItem entity);
    Task<TripGearItem?> UpdateGearItemAsync(TripGearItem entity);
    Task<TripGearItem?> GetGearItemByIdAsync(int gearItemId);
    Task<bool> DeleteGearItemAsync(int gearItemId);
    Task<bool> ToggleGearCheckAsync(int gearItemId, int userId);
    #endregion

    #region 地點
    // 地點
    Task<List<TripTripLocation>> GetLocationsAsync(int tripId);
    Task<TripTripLocation> CreateLocationAsync(TripTripLocation entity);
    Task<TripTripLocation?> UpdateLocationAsync(TripTripLocation entity);
    Task<TripTripLocation?> GetLocationByIdAsync(int locationId);
    Task<bool> DeleteLocationAsync(int locationId);
    #endregion

    #region 提醒
    // 提醒
    Task<List<TripReminder>> GetRemindersAsync(int tripId, int userId);
    Task<TripReminder> CreateReminderAsync(TripReminder entity);
    Task<TripReminder?> UpdateReminderAsync(TripReminder entity);
    Task<TripReminder?> GetReminderByIdAsync(int reminderId);
    Task<bool> ToggleReminderAsync(int reminderId);
    #endregion

    #region 城市
    // 城市
    Task<List<TripCity>> GetCitiesAsync();
    Task<List<TripDistrict>> GetDistrictsAsync(int cityId);
    #endregion

    #region 權限控管
    // 確認是否為主辦人
    Task<bool> IsOrganizerAsync(int tripId, int userId);

    // 確認是否為成員
    Task<bool> IsMemberAsync(int tripId, int userId);
    #endregion
}