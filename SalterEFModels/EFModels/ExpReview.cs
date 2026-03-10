using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpReview
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? CoachId { get; set; }

    public int? Rating { get; set; }

    public string? ReviewContent { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int? CourseOrderId { get; set; }

    public bool? IsHidden { get; set; }

    public virtual ExpCoach? Coach { get; set; }

    public virtual ExpCourseOrder? CourseOrder { get; set; }

    public virtual ICollection<ExpReport> ExpReports { get; set; } = new List<ExpReport>();

    public virtual UserUser? User { get; set; }
}
