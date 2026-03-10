using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripReminder
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public int UserId { get; set; }

    public int RemindOffsetMinutes { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime? LastSentAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TripTrip Trip { get; set; } = null!;
}
