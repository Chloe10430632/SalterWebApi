using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripNote
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public int? TimelineId { get; set; }

    public string Content { get; set; } = null!;

    public int UpdatedByUserId { get; set; }

    public int Version { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TripTimeline? Timeline { get; set; }

    public virtual TripTrip Trip { get; set; } = null!;
}
