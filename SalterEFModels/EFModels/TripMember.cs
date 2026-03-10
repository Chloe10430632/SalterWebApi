using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripMember
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual TripTrip Trip { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
