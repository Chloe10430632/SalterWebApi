using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomBooking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int RoomTypeId { get; set; }

    public int? TransactionsId { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? CheckOutDate { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string? CancelReason { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<HomReview> HomReviews { get; set; } = new List<HomReview>();

    public virtual HomRoomType RoomType { get; set; } = null!;

    public virtual ExpTransaction Transactions { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
