using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomCitieMapping
{
    public int HouseId { get; set; }

    public int CityId { get; set; }

    public virtual TripCity City { get; set; } = null!;

    public virtual HomHouse House { get; set; } = null!;
}
