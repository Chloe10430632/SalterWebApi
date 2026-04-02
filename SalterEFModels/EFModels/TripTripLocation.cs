using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalterEFModels.EFModels;

public partial class TripTripLocation
{
    public int Id { get; set; }

    public int TripId { get; set; }

    [Column("day_number")]
    public int DayNumber { get; set; }

    public int LocationId { get; set; }

    public string? LocationRole { get; set; }

    public int SortOrder { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TripLocation Location { get; set; } = null!;

    public virtual TripTrip Trip { get; set; } = null!;

    public virtual ICollection<TripTimeline> TripTimelines { get; set; } = new List<TripTimeline>();
}
