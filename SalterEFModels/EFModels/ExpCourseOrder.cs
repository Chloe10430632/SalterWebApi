using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpCourseOrder
{
    public int Id { get; set; }

    public int? CourseSessionId { get; set; }

    public DateTime? ReservedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UserId { get; set; }

    public string? Status { get; set; }

    public int? ExpTransactionId { get; set; }

    public virtual ExpCourseSession? CourseSession { get; set; }

    public virtual ICollection<ExpReview> ExpReviews { get; set; } = new List<ExpReview>();

    public virtual ExpTransaction? ExpTransaction { get; set; }

    public virtual UserUser? User { get; set; }
}
