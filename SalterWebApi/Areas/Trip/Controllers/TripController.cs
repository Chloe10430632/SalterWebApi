using Microsoft.AspNetCore.Mvc;
using TripServiceHelper.IService;
using TripServiceHelper.Models.DTOs;

namespace SalterWebApi.Areas.Trip.Controllers;

[ApiController]
[Area("Trip")]
[Route("api/trip/[controller]")]
[Tags("揪團行程")]
public class TripController : ControllerBase
{
    private readonly ITripService _service;

    public TripController(ITripService service)
    {
        _service = service;
    }

    #region 行程

    // GET api/trip/trip?keyword=台北&page=1
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] TripQueryDto query)
    {
        var data = await _service.GetTripListAsync(query);
        return Ok(ApiResponse<TripListResultDto>.Ok(data));
    }

    // GET api/trip/trip/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetTripDetailAsync(id);
        if (data == null)
            return NotFound(ApiResponse<TripDetailDto>.Fail("找不到行程"));
        return Ok(ApiResponse<TripDetailDto>.Ok(data));
    }

    // POST api/trip/trip
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TripRequestDto dto)
    {
        var organizerUserId = 1; // TODO: 從 JWT 取得
        var result = await _service.CreateTripAsync(dto, organizerUserId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PUT api/trip/trip/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TripRequestDto dto)
    {
        var result = await _service.UpdateTripAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // DELETE api/trip/trip/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteTripAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 成員

    // POST api/trip/trip/5/join
    [HttpPost("{id}/join")]
    public async Task<IActionResult> Join(int id)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.JoinTripAsync(id, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // DELETE api/trip/trip/5/leave
    [HttpDelete("{id}/leave")]
    public async Task<IActionResult> Leave(int id)
    {
        var userId = 1; //TODO:從JWT取得
        var result = await _service.LeaveTripAsync(id, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 收藏

    // GET api/trip/trip/favorites
    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = 1; // TODO: 從 JWT 取得
        var data = await _service.GetFavoritesAsync(userId);
        return Ok(ApiResponse<List<TripSummaryDto>>.Ok(data));
    }

    // POST api/trip/trip/5/favorite
    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> AddFavorite(int id)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.AddFavoriteAsync(id, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // DELETE api/trip/trip/5/favorite
    [HttpDelete("{id}/favorite")]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.RemoveFavoriteAsync(id, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));

    }
    #endregion

    #region 公告

    // GET api/trip/trip/5/announcements
    [HttpGet("{id}/announcements")]
    public async Task<IActionResult> GetAnnouncements(int id)
    {
        var data = await _service.GetAnnouncementsAsync(id);
        return Ok(ApiResponse<List<TripAnnouncementDto>>.Ok(data));
    }

    // POST api/trip/trip/5/announcements
    [HttpPost("{id}/announcements")]
    public async Task<IActionResult> CreateAnnouncement(int id, [FromBody] TripAnnouncementRequestDto dto)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.CreateAnnouncementAsync(id, dto, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PUT api/trip/trip/announcements/3
    [HttpPut("announcements/{aid}")]
    public async Task<IActionResult> UpdateAnnouncement(int aid, [FromBody] TripAnnouncementRequestDto dto)
    {
        var result = await _service.UpdateAnnouncementAsync(aid, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // DELETE api/trip/trip/announcements/3
    [HttpDelete("announcements/{aid}")]
    public async Task<IActionResult> DeleteAnnouncement(int aid)
    {
        var result = await _service.DeleteAnnouncementAsync(aid);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PATCH api/trip/trip/announcements/3/pin
    [HttpPatch("announcements/{aid}/pin")]
    public async Task<IActionResult> TogglePin(int aid)
    {
        var result = await _service.TogglePinAsync(aid);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 裝備

    // GET api/trip/trip/5/gearitems
    [HttpGet("{id}/gearitems")]
    public async Task<IActionResult> GetGearItems(int id)
    {
        var data = await _service.GetGearItemsAsync(id);
        return Ok(ApiResponse<List<TripGearItemDto>>.Ok(data));
    }

    // POST api/trip/trip/5/gearitems
    [HttpPost("{id}/gearitems")]
    public async Task<IActionResult> CreateGearItem(int id, [FromBody] TripGearItemRequestDto dto)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.CreateGearItemAsync(id, dto, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PUT api/trip/trip/gearitems/3
    [HttpPut("gearitems/{gid}")]
    public async Task<IActionResult> UpdateGearItem(int gid, [FromBody] TripGearItemRequestDto dto)
    {
        var result = await _service.UpdateGearItemAsync(gid, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // DELETE api/trip/trip/gearitems/3
    [HttpDelete("gearitems/{gid}")]
    public async Task<IActionResult> DeleteGearItem(int gid)
    {
        var result = await _service.DeleteGearItemAsync(gid);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // POST api/trip/trip/gearitems/3/check
    [HttpPost("gearitems/{gid}/check")]
    public async Task<IActionResult> ToggleGearCheck(int gid)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.ToggleGearCheckAsync(gid, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 地點

    // GET api/trip/trip/5/locations
    [HttpGet("{id}/locations")]
    public async Task<IActionResult> GetLocations(int id)
    {
        var data = await _service.GetLocationsAsync(id);
        return Ok(ApiResponse<List<TripLocationDto>>.Ok(data));
    }

    // POST api/trip/trip/5/locations
    [HttpPost("{id}/locations")]
    public async Task<IActionResult> CreateLocation(int id, [FromBody] TripLocationRequestDto dto)
    {
        var result = await _service.CreateLocationAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PUT api/trip/trip/locations/3
    [HttpPut("locations/{lid}")]
    public async Task<IActionResult> UpdateLocation(int lid, [FromBody] TripLocationRequestDto dto)
    {
        var result = await _service.UpdateLocationAsync(lid, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // DELETE api/trip/trip/locations/3
    [HttpDelete("locations/{lid}")]
    public async Task<IActionResult> DeleteLocation(int lid)
    {
        var result = await _service.DeleteLocationAsync(lid);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 提醒

    // GET api/trip/trip/5/reminders
    [HttpGet("{id}/reminders")]
    public async Task<IActionResult> GetReminders(int id)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var data = await _service.GetRemindersAsync(id, userId);
        return Ok(ApiResponse<List<TripReminderDto>>.Ok(data));
    }

    // POST api/trip/trip/5/reminders
    [HttpPost("{id}/reminders")]
    public async Task<IActionResult> CreateReminder(int id, [FromBody] TripReminderRequestDto dto)
    {
        var userId = 1; // TODO: 從 JWT 取得
        var result = await _service.CreateReminderAsync(id, dto, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PUT api/trip/trip/reminders/3
    [HttpPut("reminders/{rid}")]
    public async Task<IActionResult> UpdateReminder(int rid, [FromBody] TripReminderRequestDto dto)
    {
        var result = await _service.UpdateReminderAsync(rid, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    // PATCH api/trip/trip/reminders/3/toggle
    [HttpPatch("reminders/{rid}/toggle")]
    public async Task<IActionResult> ToggleReminder(int rid)
    {
        var result = await _service.ToggleReminderAsync(rid);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 城市

    // GET api/trip/trip/cities
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var data = await _service.GetCitiesAsync();
        return Ok(ApiResponse<List<TripCityDto>>.Ok(data));
    }

    // GET api/trip/trip/cities/1/districts
    [HttpGet("cities/{cityId}/districts")]
    public async Task<IActionResult> GetDistricts(int cityId)
    {
        var data = await _service.GetDistrictsAsync(cityId);
        return Ok(ApiResponse<List<TripDistrictDto>>.Ok(data));
    }

    #endregion
}