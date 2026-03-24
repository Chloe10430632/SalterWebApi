using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripLocation
{
    public int Id { get; set; }

    public string? GooglePlaceId { get; set; }

    public int CityId { get; set; }

    public int DistrictId { get; set; }

    public string Name { get; set; } = null!;

    public string AddressText { get; set; } = null!;

    public decimal Lat { get; set; }

    public decimal Lng { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ExpCourseTemplate> ExpCourseTemplates { get; set; } = new List<ExpCourseTemplate>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<TripTripLocation> TripTripLocations { get; set; } = new List<TripTripLocation>();
}
