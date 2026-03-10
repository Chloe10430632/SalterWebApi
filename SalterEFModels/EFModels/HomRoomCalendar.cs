using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomRoomCalendar
{
    public int CalendarId { get; set; }

    public int RoomTypeId { get; set; }

    public DateOnly? TargetDate { get; set; }

    public decimal? Price { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual HomRoomType RoomType { get; set; } = null!;
}
