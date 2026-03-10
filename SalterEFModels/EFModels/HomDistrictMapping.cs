using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomDistrictMapping
{
    public int HouseId { get; set; }

    public int DistrictId { get; set; }

    public virtual TripDistrict District { get; set; } = null!;

    public virtual HomHouse House { get; set; } = null!;
}
