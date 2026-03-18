using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TripServiceHelper.IService;
using TripServiceHelper.Models.DTOs;

namespace SalterWebApi.Areas.Trip.Controllers;

[Authorize]
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

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] TripQueryDto query)
    {
        var data = await _service.GetTripListAsync(query);
        return Ok(ApiResponse<TripListResultDto>.Ok(data));
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _service.GetTripDetailAsync(id);
        if (data == null)
            return NotFound(ApiResponse<TripDetailDto>.Fail("找不到行程", 404));
        return Ok(ApiResponse<TripDetailDto>.Ok(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TripRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.CreateTripAsync(dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TripRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.UpdateTripAsync(id, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.DeleteTripAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 成員

    [HttpPost("{id}/join")]
    public async Task<IActionResult> Join(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.JoinTripAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpDelete("{id}/leave")]
    public async Task<IActionResult> Leave(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.LeaveTripAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 收藏

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var data = await _service.GetFavoritesAsync(userId.Value);
        return Ok(ApiResponse<List<TripSummaryDto>>.Ok(data));
    }

    [HttpPost("{id}/favorite")]
    public async Task<IActionResult> AddFavorite(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.AddFavoriteAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpDelete("{id}/favorite")]
    public async Task<IActionResult> RemoveFavorite(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.RemoveFavoriteAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 公告

    [HttpGet("{id}/announcements")]
    public async Task<IActionResult> GetAnnouncements(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.GetAnnouncementsAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<List<TripAnnouncementDto>>.Ok(result.Data!));
    }

    [HttpPost("{id}/announcements")]
    public async Task<IActionResult> CreateAnnouncement(int id, [FromBody] TripAnnouncementRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.CreateAnnouncementAsync(id, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPut("announcements/{aid}")]
    public async Task<IActionResult> UpdateAnnouncement(int aid, [FromBody] TripAnnouncementUpdateDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.UpdateAnnouncementAsync(aid, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }
    [HttpDelete("announcements/{aid}")]
    public async Task<IActionResult> DeleteAnnouncement(int aid)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.DeleteAnnouncementAsync(aid, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPatch("announcements/{aid}/pin")]
    public async Task<IActionResult> TogglePin(int aid)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.TogglePinAsync(aid, userId.Value);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message, 404));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 裝備

    [HttpGet("{id}/gearitems")]
    public async Task<IActionResult> GetGearItems(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.GetGearItemsAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<List<TripGearItemDto>>.Ok(result.Data!));
    }

    [HttpPost("{id}/gearitems")]
    public async Task<IActionResult> CreateGearItem(int id, [FromBody] TripGearItemRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.CreateGearItemAsync(id, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPut("gearitems/{gid}")]
    public async Task<IActionResult> UpdateGearItem(int gid, [FromBody] TripGearItemRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.UpdateGearItemAsync(gid, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpDelete("gearitems/{gid}")]
    public async Task<IActionResult> DeleteGearItem(int gid)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.DeleteGearItemAsync(gid, userId.Value);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message, 404));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPost("gearitems/{gid}/check")]
    public async Task<IActionResult> ToggleGearCheck(int gid)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.ToggleGearCheckAsync(gid, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 地點

    [HttpGet("{id}/locations")]
    public async Task<IActionResult> GetLocations(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.GetLocationsAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<List<TripLocationDto>>.Ok(result.Data!));
    }

    [HttpPost("{id}/locations")]
    public async Task<IActionResult> CreateLocation(int id, [FromBody] TripLocationRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.CreateLocationAsync(id, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPut("locations/{lid}")]
    public async Task<IActionResult> UpdateLocation(int lid, [FromBody] TripLocationUpdateDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.UpdateLocationAsync(lid, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpDelete("locations/{lid}")]
    public async Task<IActionResult> DeleteLocation(int lid)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.DeleteLocationAsync(lid, userId.Value);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message, 404));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 提醒

    [HttpGet("{id}/reminders")]
    public async Task<IActionResult> GetReminders(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.GetRemindersAsync(id, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<List<TripReminderDto>>.Ok(result.Data!));
    }

    [HttpPost("{id}/reminders")]
    public async Task<IActionResult> CreateReminder(int id, [FromBody] TripReminderRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.CreateReminderAsync(id, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPut("reminders/{rid}")]
    public async Task<IActionResult> UpdateReminder(int rid, [FromBody] TripReminderRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("無效的憑證", 401));
        var result = await _service.UpdateReminderAsync(rid, dto, userId.Value);
        if (!result.IsSuccess)
            return StatusCode(result.Code, ApiResponse<string>.Fail(result.Message, result.Code));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    [HttpPatch("reminders/{rid}/toggle")]
    public async Task<IActionResult> ToggleReminder(int rid)
    {
        var result = await _service.ToggleReminderAsync(rid);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<string>.Fail(result.Message, 404));
        return Ok(ApiResponse<string>.Ok(result.Message));
    }

    #endregion

    #region 城市

    [AllowAnonymous]
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var data = await _service.GetCitiesAsync();
        return Ok(ApiResponse<List<TripCityDto>>.Ok(data));
    }

    [AllowAnonymous]
    [HttpGet("cities/{cityId}/districts")]
    public async Task<IActionResult> GetDistricts(int cityId)
    {
        var data = await _service.GetDistrictsAsync(cityId);
        return Ok(ApiResponse<List<TripDistrictDto>>.Ok(data));
    }

    #endregion

    #region 方法

    private int? GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return null;
        return int.Parse(userIdStr);
    }

    #endregion
}