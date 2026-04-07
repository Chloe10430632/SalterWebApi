using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpCourseSession
{
    public int Id { get; set; }

    public int? CourseTemplateId { get; set; }

    public int? CoachId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? TimeSlot { get; set; }

    public int? MaxParticipants { get; set; }

    public int? CurrentParticipants { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ExpCourseTemplate? CourseTemplate { get; set; }

    public virtual ICollection<ExpCourseOrder> ExpCourseOrders { get; set; } = new List<ExpCourseOrder>();
}
