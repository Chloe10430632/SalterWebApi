using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripGearCheck
{
    public int Id { get; set; }

    public int TripGearItemId { get; set; }

    public int UserId { get; set; }

    public bool IsChecked { get; set; }

    public DateTime? CheckedAt { get; set; }

    public virtual TripGearItem TripGearItem { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
