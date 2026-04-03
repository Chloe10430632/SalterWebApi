using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripFavorite
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? FolderId { get; set; }

    public virtual TripFavoriteFolder? Folder { get; set; }

    public virtual TripTrip Trip { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
