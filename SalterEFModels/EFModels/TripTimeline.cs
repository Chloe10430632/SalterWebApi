using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripTimeline
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public int? TripLocationId { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TripTrip Trip { get; set; } = null!;

    public virtual TripTripLocation? TripLocation { get; set; }

    public virtual ICollection<TripNote> TripNotes { get; set; } = new List<TripNote>();
}
