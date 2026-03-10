using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomAmenity
{
    public int AmenityId { get; set; }

    public string? Name { get; set; }

    public string? IconCode { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<HomRoomTypeAmenity> HomRoomTypeAmenities { get; set; } = new List<HomRoomTypeAmenity>();
}
