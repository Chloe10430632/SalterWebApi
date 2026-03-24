using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripCity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<TripDistrict> TripDistricts { get; set; } = new List<TripDistrict>();

    public virtual ICollection<TripLocation> TripLocations { get; set; } = new List<TripLocation>();
}
