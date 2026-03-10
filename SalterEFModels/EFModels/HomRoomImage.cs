using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomRoomImage
{
    public int ImageId { get; set; }

    public int RoomTypeId { get; set; }

    public string? ImagePath { get; set; }

    public DateTime? CreatedTime { get; set; }

    public virtual HomRoomType RoomType { get; set; } = null!;
}
