using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripGearItem
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string ItemName { get; set; } = null!;

    public bool IsRequired { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual UserUser? CreatedByUser { get; set; }

    public virtual TripTrip Trip { get; set; } = null!;

    public virtual ICollection<TripGearCheck> TripGearChecks { get; set; } = new List<TripGearCheck>();
}
