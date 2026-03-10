using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpReport
{
    public int Id { get; set; }

    public int? ReporterId { get; set; }

    public int? TargetCoachId { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? HandledByAdminId { get; set; }

    public int? TargetReviewId { get; set; }

    public virtual UserAdmin? HandledByAdmin { get; set; }

    public virtual ExpReview? TargetReview { get; set; }
}
