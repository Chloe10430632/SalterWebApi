using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomRoomTypeAmenity
{
    public int RoomTypeAmenityId { get; set; }

    public int RoomTypeId { get; set; }

    public int AmenityId { get; set; }

    public virtual HomAmenity Amenity { get; set; } = null!;

    public virtual HomRoomType RoomType { get; set; } = null!;
}
