using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpCoach
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Introduction { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? LocationId { get; set; }

    public int UserId { get; set; }

    public bool? IsSuspended { get; set; }

    public int? DistrictId { get; set; }

    public int? CityId { get; set; }

    public virtual TripCity? City { get; set; }

    public virtual ICollection<ExpCourseTemplate> ExpCourseTemplates { get; set; } = new List<ExpCourseTemplate>();

    public virtual ICollection<ExpEquipment> ExpEquipments { get; set; } = new List<ExpEquipment>();

    public virtual ICollection<ExpReview> ExpReviews { get; set; } = new List<ExpReview>();

    public virtual UserUser User { get; set; } = null!;

    public virtual ICollection<TripDistrict> TripDistricts { get; set; } = new List<TripDistrict>();
}
