using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomLocationMapping
{
    public int HouseId { get; set; }

    public int LocationId { get; set; }

    public virtual HomHouse House { get; set; } = null!;

    public virtual TripLocation Location { get; set; } = null!;
}
