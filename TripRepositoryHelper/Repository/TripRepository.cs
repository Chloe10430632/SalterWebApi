using Microsoft.EntityFrameworkCore;
using SalterEFModels.EFModels;
using TripRepositoryHelper.IRepository;

namespace TripRepositoryHelper.Repository;

public class TripRepository : ITripRepository
{
    private readonly SalterDbContext _db;

    public TripRepository(SalterDbContext db)
    {
        _db = db;
    }

    #region 行程

    public async Task<(List<TripTrip> trips, int totalCount)> GetTripsAsync(
        string? keyword, string? tripType, string? status, int? cityId,
        DateTime? startFrom, DateTime? startTo,
        int? minCapacity, int? maxCapacity,
        string? sortBy, int page, int pageSize)
    {
        var q = _db.TripTrips
            .Include(t => t.OrganizerUser)
            .Include(t => t.TripMembers)
            .Include(t => t.TripFavorites)
            .Include(t => t.TripTripLocations)
                .ThenInclude(ttl => ttl.Location)
                    //.ThenInclude(l => l.District)
                        //.ThenInclude(d => d.City)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            q = q.Where(t => t.Title.Contains(keyword) ||
                (t.Description != null && t.Description.Contains(keyword)));
        if (!string.IsNullOrEmpty(tripType))
            q = q.Where(t => t.TripType == tripType);
        if (!string.IsNullOrEmpty(status))
        {
            var statuses = status.Split(',', StringSplitOptions.RemoveEmptyEntries);
            q = q.Where(t => statuses.Contains(t.Status));
        }
        if (cityId.HasValue)
            q = q.Where(t => t.TripTripLocations.Any(
                ttl => ttl.Location != null && ttl.Location.CityId == cityId.Value));
        if (startFrom.HasValue)
            q = q.Where(t => t.StartAt >= startFrom.Value);
        if (startTo.HasValue)
            q = q.Where(t => t.StartAt <= startTo.Value);
        if (minCapacity.HasValue)
            q = q.Where(t => t.Capacity >= minCapacity.Value);
        if (maxCapacity.HasValue)
            q = q.Where(t => t.Capacity <= maxCapacity.Value);

        q = sortBy switch
        {
            "startAt" => q.OrderBy(t => t.StartAt),
            "capacity" => q.OrderByDescending(t => t.Capacity),
            "popular" => q.OrderByDescending(t => t.TripFavorites.Count),
            _ => q.OrderByDescending(t => t.CreatedAt)
        };

        int totalCount = await q.CountAsync();
        var trips = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (trips, totalCount);
    }

    public async Task<TripTrip?> GetTripByIdAsync(int tripId)
    {
        return await _db.TripTrips
            .Include(t => t.OrganizerUser)
            .Include(t => t.TripMembers).ThenInclude(m => m.User)
            .Include(t => t.TripFavorites)
            .Include(t => t.TripAnnouncements)
            .Include(t => t.TripGearItems)
            .Include(t => t.TripTripLocations)
                .ThenInclude(ttl => ttl.Location)
                    //.ThenInclude(l => l.District)
                        //.ThenInclude(d => d.City)
            .FirstOrDefaultAsync(t => t.Id == tripId);
    }

    public async Task<TripTrip> CreateTripAsync(TripTrip trip)
    {
        _db.TripTrips.Add(trip);
        await _db.SaveChangesAsync();
        return trip;
    }

    public async Task<TripTrip?> UpdateTripAsync(TripTrip trip)
    {
        _db.TripTrips.Update(trip);
        await _db.SaveChangesAsync();
        return trip;
    }

    public async Task<bool> DeleteTripAsync(int tripId)
    {
        var entity = await _db.TripTrips
            .Include(t => t.TripMembers)
            .Include(t => t.TripFavorites)
            .Include(t => t.TripAnnouncements)
            .Include(t => t.TripGearItems)
                .ThenInclude(g => g.TripGearChecks)
            .Include(t => t.TripTripLocations)
            .FirstOrDefaultAsync(t => t.Id == tripId);

        if (entity == null) return false;

        _db.TripMembers.RemoveRange(entity.TripMembers);
        _db.TripFavorites.RemoveRange(entity.TripFavorites);
        _db.TripAnnouncements.RemoveRange(entity.TripAnnouncements);
        _db.TripGearChecks.RemoveRange(entity.TripGearItems.SelectMany(g => g.TripGearChecks));
        _db.TripGearItems.RemoveRange(entity.TripGearItems);
        _db.TripTripLocations.RemoveRange(entity.TripTripLocations);

        _db.TripTrips.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<TripTrip>> GetMyTripsAsync(int userId, string? role = null)
    {
        var q = _db.TripMembers
            .Where(m => m.UserId == userId)
            .Include(m => m.Trip)
                .ThenInclude(t => t.OrganizerUser)
            .Include(m => m.Trip)
                .ThenInclude(t => t.TripMembers)
            .Include(m => m.Trip)
                .ThenInclude(t => t.TripFavorites)
            .AsQueryable();

        if (!string.IsNullOrEmpty(role))
            q = q.Where(m => m.Role == role);

        return await q
            .OrderByDescending(m => m.Trip.StartAt)
            .Select(m => m.Trip)
            .ToListAsync();
    }

    #endregion

    #region 成員

    public async Task<bool> AddMemberAsync(int tripId, int userId, string role)
    {
        var exists = await _db.TripMembers
            .AnyAsync(m => m.TripId == tripId && m.UserId == userId);
        if (exists) return false;

        _db.TripMembers.Add(new TripMember
        {
            TripId = tripId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveMemberAsync(int tripId, int userId)
    {
        var entity = await _db.TripMembers
            .FirstOrDefaultAsync(m => m.TripId == tripId && m.UserId == userId);
        if (entity == null) return false;
        _db.TripMembers.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 收藏

    public async Task<List<TripFavorite>> GetFavoritesAsync(int userId)
    {
        return await _db.TripFavorites
            .Include(f => f.Trip)
                .ThenInclude(t => t.TripMembers)  
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }
    public async Task<bool> AddFavoriteAsync(int tripId, int userId)
    {
        var exists = await _db.TripFavorites
            .AnyAsync(f => f.TripId == tripId && f.UserId == userId);
        if (exists) return false;

        _db.TripFavorites.Add(new TripFavorite
        {
            TripId = tripId,
            UserId = userId,
            CreatedAt = DateTime.Now
        });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(int tripId, int userId)
    {
        var entity = await _db.TripFavorites
            .FirstOrDefaultAsync(f => f.TripId == tripId && f.UserId == userId);
        if (entity == null) return false;
        _db.TripFavorites.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 收藏資料夾
    public async Task<List<TripFavoriteFolder>> GetFoldersAsync(int userId)
    {
        return await _db.TripFavoriteFolders
            .Where(f => f.UserId == userId)
            .Include(f => f.TripFavorites)
            .OrderBy(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<TripFavoriteFolder?> GetFolderByIdAsync(int folderId)
    {
        return await _db.TripFavoriteFolders
            .FirstOrDefaultAsync(f => f.Id == folderId);
    }

    public async Task<TripFavoriteFolder> CreateFolderAsync(TripFavoriteFolder folder)
    {
        _db.TripFavoriteFolders.Add(folder);
        await _db.SaveChangesAsync();
        return folder;
    }

    public async Task UpdateFolderAsync(TripFavoriteFolder folder)
    {
        _db.TripFavoriteFolders.Update(folder);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteFolderAsync(int folderId)
    {
        // 先把資料夾內的收藏移到未分類
        var favorites = await _db.TripFavorites
            .Where(f => f.FolderId == folderId)
            .ToListAsync();
        favorites.ForEach(f => f.FolderId = null);
        await _db.SaveChangesAsync();

        var folder = await _db.TripFavoriteFolders.FindAsync(folderId);
        if (folder != null)
        {
            _db.TripFavoriteFolders.Remove(folder);
            await _db.SaveChangesAsync();
        }
    }

    public async Task MoveFavoriteToFolderAsync(int tripId, int userId, int? folderId)
    {
        var favorite = await _db.TripFavorites
            .FirstOrDefaultAsync(f => f.TripId == tripId && f.UserId == userId);
        if (favorite != null)
        {
            favorite.FolderId = folderId;
            await _db.SaveChangesAsync();
        }
    }
#endregion

    #region 公告

    public async Task<List<TripAnnouncement>> GetAnnouncementsAsync(int tripId)
    {
        return await _db.TripAnnouncements
            .Where(a => a.TripId == tripId)
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<TripAnnouncement> CreateAnnouncementAsync(TripAnnouncement entity)
    {
        _db.TripAnnouncements.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<TripAnnouncement?> UpdateAnnouncementAsync(TripAnnouncement entity)
    {
        _db.TripAnnouncements.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    public async Task<TripAnnouncement?> GetAnnouncementByIdAsync(int announcementId)
    => await _db.TripAnnouncements.FindAsync(announcementId);

    public async Task<bool> DeleteAnnouncementAsync(int announcementId)
    {
        var entity = await _db.TripAnnouncements.FindAsync(announcementId);
        if (entity == null) return false;
        _db.TripAnnouncements.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TogglePinAnnouncementAsync(int announcementId)
    {
        var entity = await _db.TripAnnouncements.FindAsync(announcementId);
        if (entity == null) return false;
        entity.IsPinned = !entity.IsPinned;
        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 裝備清單

    public async Task<List<TripGearItem>> GetGearItemsAsync(int tripId)
    {
        return await _db.TripGearItems
            .Include(g => g.TripGearChecks)
            .ThenInclude(c => c.User)
            .Where(g => g.TripId == tripId)
            .ToListAsync();
    }

    public async Task<TripGearItem> CreateGearItemAsync(TripGearItem entity)
    {
        _db.TripGearItems.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<TripGearItem?> UpdateGearItemAsync(TripGearItem entity)
    {
        _db.TripGearItems.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
    public async Task<TripGearItem?> GetGearItemByIdAsync(int gearItemId)
    => await _db.TripGearItems.FindAsync(gearItemId);

    public async Task DeleteGearItemAsync(int gearItemId)
    {
        var checks = _db.TripGearChecks.Where(c => c.TripGearItemId == gearItemId);
        _db.TripGearChecks.RemoveRange(checks);

        var entity = await _db.TripGearItems.FindAsync(gearItemId);
        if (entity != null) _db.TripGearItems.Remove(entity);

        await _db.SaveChangesAsync();
    }

    public async Task<bool> ToggleGearCheckAsync(int gearItemId, int userId)
    {
        var entity = await _db.TripGearChecks
            .FirstOrDefaultAsync(g => g.TripGearItemId == gearItemId && g.UserId == userId);

        if (entity == null)
        {
            _db.TripGearChecks.Add(new TripGearCheck
            {
                TripGearItemId = gearItemId,
                UserId = userId,
                IsChecked = true,
                CheckedAt = DateTime.Now
            });
        }
        else
        {
            entity.IsChecked = !entity.IsChecked;
            entity.CheckedAt = DateTime.Now;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    #endregion

    #region 地點

    public async Task<List<TripTripLocation>> GetLocationsAsync(int tripId)
    {
        return await _db.TripTripLocations
            .Include(ttl => ttl.Location)
                //.ThenInclude(l => l.District)
                //    .ThenInclude(d => d.City)
            .Where(ttl => ttl.TripId == tripId)
            .OrderBy(ttl => ttl.SortOrder)
            .ToListAsync();
    }

    public async Task<TripTripLocation> CreateLocationAsync(TripTripLocation entity)
    {
        _db.TripTripLocations.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<TripTripLocation?> UpdateLocationAsync(TripTripLocation entity)
    {
        _db.TripTripLocations.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteLocationAsync(int locationId)
    {
        var entity = await _db.TripTripLocations.FindAsync(locationId);
        if (entity == null) return false;
        _db.TripTripLocations.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
    public async Task<TripTripLocation?> GetLocationByIdAsync(int locationId)
    => await _db.TripTripLocations.FindAsync(locationId);

    public async Task<List<TripLocation>> GetAllLocationsAsync(string? keyword)
    {
        var q = _db.TripLocations
            //.Include(l => l.District)
            //    .ThenInclude(d => d.City)
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            q = q.Where(l => l.Name.Contains(keyword));

        return await q.OrderBy(l => l.Name).ToListAsync();
    }
    public async Task<TripCity?> GetCityByNameAsync(string name)
    => await _db.TripCities.FirstOrDefaultAsync(c => c.Name == name);

    public async Task<TripDistrict?> GetDistrictByNameAsync(string name, int cityId)
        => await _db.TripDistricts.FirstOrDefaultAsync(d => d.Name == name && d.CityId == cityId);

    public async Task<TripLocation?> GetLocationByGooglePlaceIdAsync(string? placeId)
    {
        if (string.IsNullOrEmpty(placeId)) return null;
        return await _db.TripLocations.FirstOrDefaultAsync(l => l.GooglePlaceId == placeId);
    }

    public async Task<TripLocation> CreateTripLocationAsync(TripLocation location)
    {
        _db.TripLocations.Add(location);
        await _db.SaveChangesAsync();
        return location;
    }
    public async Task<TripCity> CreateCityAsync(TripCity city)
    {
        _db.TripCities.Add(city);
        await _db.SaveChangesAsync();
        return city;
    }

    public async Task<TripDistrict> CreateDistrictAsync(TripDistrict district)
    {
        _db.TripDistricts.Add(district);
        await _db.SaveChangesAsync();
        return district;
    }
    public async Task UpdateLocationSortAsync(List<(int locationId, int sortOrder)> items)
    {
        // 先設成負數避免衝突
        foreach (var (locationId, _) in items)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "UPDATE TripTripLocations SET sort_order = -id WHERE id = {0}", locationId);
        }
        // 再設成正確的 sortOrder
        foreach (var (locationId, sortOrder) in items)
        {
            await _db.Database.ExecuteSqlRawAsync(
                "UPDATE TripTripLocations SET sort_order = {0}, updated_at = {1} WHERE id = {2}",
                sortOrder, DateTime.Now, locationId);
        }
    }
    public async Task<int> GetMaxSortOrderAsync(int tripId, int dayNumber)
    {
        return await _db.TripTripLocations
            .Where(t => t.TripId == tripId && t.DayNumber == dayNumber)
            .MaxAsync(t => (int?)t.SortOrder) ?? 0;
    }
    #endregion

    #region 城市

    public async Task<List<TripCity>> GetCitiesAsync()
    {
        return await _db.TripCities.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<List<TripDistrict>> GetDistrictsAsync(int cityId)
    {
        return await _db.TripDistricts
            .Where(d => d.CityId == cityId)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    #endregion

    #region 權限控管
    public async Task<bool> IsOrganizerAsync(int tripId, int userId)
    {
        return await _db.TripTrips
            .AnyAsync(t => t.Id == tripId && t.OrganizerUserId == userId);
    }

    public async Task<bool> IsMemberAsync(int tripId, int userId)
    {
        return await _db.TripMembers
            .AnyAsync(m => m.TripId == tripId && m.UserId == userId);
    }
    #endregion
}