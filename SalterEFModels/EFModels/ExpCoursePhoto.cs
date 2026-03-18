using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpCoursePhoto
{
    public int Id { get; set; }

    public int? CourseTemplateId { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual ICollection<ExpCourseTemplate> ExpCourseTemplates { get; set; } = new List<ExpCourseTemplate>();
}
