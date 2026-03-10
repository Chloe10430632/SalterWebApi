using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumAd
{
    public int AdId { get; set; }

    public string Title { get; set; } = null!;

    public int UserId { get; set; }

    public string ImgUrl { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public int TransactionId { get; set; }

    public virtual ExpTransaction Transaction { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
