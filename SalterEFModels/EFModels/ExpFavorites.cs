using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpFavorites
{
    public int? UserId { get; set; }

    public int? CoachId { get; set; }

    public DateTime? FavoritedAt { get; set; }

    public virtual ExpCoach? Coach { get; set; }

    public virtual UserUser? User { get; set; }
}
