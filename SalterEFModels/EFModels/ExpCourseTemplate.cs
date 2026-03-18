using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpCourseTemplate
{
    public int Id { get; set; }

    public int? CoachId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? Location { get; set; }

    public string? Difficulty { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? PhotoId { get; set; }

    public int? LocationId { get; set; }

    public virtual ExpCoach? Coach { get; set; }

    public virtual ICollection<ExpCourseSession> ExpCourseSessions { get; set; } = new List<ExpCourseSession>();

    public virtual TripLocation? LocationNavigation { get; set; }

    public virtual ExpCoursePhoto? Photo { get; set; }
}
