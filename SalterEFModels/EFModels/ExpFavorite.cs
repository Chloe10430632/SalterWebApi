using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpFavorite
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CoachId { get; set; }

    public DateTime? FavoritedAt { get; set; }

    public virtual ExpCoach Coach { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
