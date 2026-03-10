using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripAnnouncement
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsPinned { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UserUser CreatedByUser { get; set; } = null!;

    public virtual TripTrip Trip { get; set; } = null!;
}
