using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpTransaction
{
    public int Id { get; set; }

    public int? SenderUserId { get; set; }

    public int? ReceiveUserId { get; set; }

    public int? Status { get; set; }

    public int? TypeId { get; set; }

    public int? Flow { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ExpCourseOrder> ExpCourseOrders { get; set; } = new List<ExpCourseOrder>();

    public virtual ICollection<ExpEquipmentOrder> ExpEquipmentOrders { get; set; } = new List<ExpEquipmentOrder>();

    public virtual ICollection<ExpTthirdPartyPayment> ExpTthirdPartyPayments { get; set; } = new List<ExpTthirdPartyPayment>();

    public virtual ICollection<ForumAd> ForumAds { get; set; } = new List<ForumAd>();

    public virtual ICollection<HomBooking> HomBookings { get; set; } = new List<HomBooking>();

    public virtual UserUser? ReceiveUser { get; set; }

    public virtual UserUser? SenderUser { get; set; }

    public virtual ExpTransactionsType? Type { get; set; }
}
