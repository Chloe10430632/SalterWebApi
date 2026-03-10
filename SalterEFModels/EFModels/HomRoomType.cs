using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomRoomType
{
    public int RoomTypeId { get; set; }

    public int HouseId { get; set; }

    public string? Name { get; set; }

    public int? Capacity { get; set; }

    public decimal? PricePerNight { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public bool? IsActive { get; set; }

    public int? ViewCount { get; set; }

    public string? Description { get; set; }

    public int? ReviewStatus { get; set; }

    public string? RejectReason { get; set; }

    public DateTime? ApplyTime { get; set; }

    public virtual ICollection<HomBooking> HomBookings { get; set; } = new List<HomBooking>();

    public virtual ICollection<HomReview> HomReviews { get; set; } = new List<HomReview>();

    public virtual ICollection<HomRoomCalendar> HomRoomCalendars { get; set; } = new List<HomRoomCalendar>();

    public virtual ICollection<HomRoomImage> HomRoomImages { get; set; } = new List<HomRoomImage>();

    public virtual ICollection<HomRoomTypeAmenity> HomRoomTypeAmenities { get; set; } = new List<HomRoomTypeAmenity>();

    public virtual HomHouse House { get; set; } = null!;
}
