using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomReview
{
    public int ReviewId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int RoomTypeId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedTime { get; set; }

    public virtual HomBooking Booking { get; set; } = null!;

    public virtual HomRoomType RoomType { get; set; } = null!;
}
