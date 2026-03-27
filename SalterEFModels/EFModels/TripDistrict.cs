using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripDistrict
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TripCity City { get; set; } = null!;

    public virtual ICollection<ExpCoach> ExpCoaches { get; set; } = new List<ExpCoach>();

    public virtual ICollection<ExpCoach> CoachDists { get; set; } = new List<ExpCoach>();
}
