using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomHouse
{
    public int HouseId { get; set; }

    public int UserId { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public string? District { get; set; }

    public string? Citie { get; set; }

    public virtual ICollection<HomRoomType> HomRoomTypes { get; set; } = new List<HomRoomType>();
}
