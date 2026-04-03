using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripFavoriteFolder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<TripFavorite> TripFavorites { get; set; } = new List<TripFavorite>();

    public virtual UserUser User { get; set; } = null!;
}
