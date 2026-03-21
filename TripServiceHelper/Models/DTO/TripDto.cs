using System.ComponentModel.DataAnnotations;

namespace TripServiceHelper.Models.DTOs;

#region 列表用
public class TripSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string TripType { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public int Capacity { get; set; }
    public int MemberCount { get; set; }
    public string Status { get; set; } = null!;
    public string? CoverImageUrl { get; set; }
    public string OrganizerName { get; set; } = null!;
    public int FavoriteCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
#endregion

#region 詳情用
public class TripDetailDto : TripSummaryDto
{
    public int OrganizerUserId { get; set; }
    public string? OrganizerEmail { get; set; }
    public DateTime? LockedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TripMemberDto> Members { get; set; } = new();
    public List<TripLocationDto> Locations { get; set; } = new();
    public int AnnouncementCount { get; set; }
    public int GearItemCount { get; set; }
}
#endregion

#region 建立／編輯用
public class TripRequestDto
{
    [Required(ErrorMessage = "標題不能為空")]
    [MaxLength(200, ErrorMessage = "標題最多 200 字")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "行程類型不能為空")]
    [MaxLength(50, ErrorMessage = "行程類型最多 50 字")]
    public string TripType { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "開始日期不能為空")]
    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    [Range(1, 9999, ErrorMessage = "人數至少 1 人")]
    public int Capacity { get; set; }

    public string? CoverImageUrl { get; set; }
    public string? CoverImagePublicId { get; set; }
}
#endregion

#region 成員
public class TripMemberDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public string Role { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
#endregion

#region 地點
public class TripLocationDto
{
    public int Id { get; set; }
    public string LocationName { get; set; } = null!;
    public string? CityName { get; set; }
    public string? DistrictName { get; set; }
    public string? LocationRole { get; set; }
    public string? Note { get; set; }
    public int SortOrder { get; set; }
}

public class TripLocationSearchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? AddressText { get; set; }
    public string? CityName { get; set; }
    public string? DistrictName { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
}
#endregion

#region 列表結果（含分頁）
public class TripListResultDto
{
    public List<TripSummaryDto> Trips { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
#endregion

#region 公告
public class TripAnnouncementDto //後端回傳給前端
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public bool IsPinned { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

//建立用
public class TripAnnouncementRequestDto
{
    [Required(ErrorMessage = "標題不能為空")]
    [MaxLength(200, ErrorMessage = "標題最多 200 字")]
    public string Title { get; set; } = null!;

    public string? Content { get; set; }
}

public class TripAnnouncementUpdateDto
{
    [MaxLength(200, ErrorMessage = "標題最多 200 字")]
    public string? Title { get; set; }
    public string? Content { get; set; }
}   
#endregion

#region 裝備
public class TripGearItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = null!;
    public bool IsRequired { get; set; }
    public bool IsCheckedByMe { get; set; }
    public int CheckedCount { get; set; }
    public List<CheckedMemberDto> CheckedMembers { get; set; } = new();
}

public class TripGearItemRequestDto
{
    [Required(ErrorMessage = "裝備名稱不能為空")]
    [MaxLength(100, ErrorMessage = "裝備名稱最多 100 字")]
    public string ItemName { get; set; } = null!;

    public bool IsRequired { get; set; }
}
public class CheckedMemberDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
    public bool IsChecked { get; set; }
}
#endregion

#region 提醒
public class TripReminderDto
{
    public int Id { get; set; }
    public int RemindOffsetMinutes { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? LastSentAt { get; set; }
}

public class TripReminderRequestDto
{
    [Range(1, 43200, ErrorMessage = "提醒時間需介於 1 分鐘到 30 天")]
    public int RemindOffsetMinutes { get; set; }

    public bool IsEnabled { get; set; }
}
#endregion

#region 地點 Request
// 建立用
public class TripLocationRequestDto
{
    public int LocationId { get; set; }
    public string? LocationRole { get; set; }
    public string? Note { get; set; }
    public int SortOrder { get; set; }
}

// 更新用
public class TripLocationUpdateDto
{
    public string? LocationRole { get; set; }
    public string? Note { get; set; }
    public int? SortOrder { get; set; }
}
#endregion

#region 城市／行政區
public class TripCityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class TripDistrictDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
#endregion