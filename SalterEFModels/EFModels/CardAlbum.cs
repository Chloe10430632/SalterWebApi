using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardAlbum
{
    public int AlbumId { get; set; }

    public int UserId { get; set; }

    public string? AlbumName { get; set; }

    public string? CoverPhoto { get; set; }

    public bool IsEnable { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CardPhoto> CardPhotos { get; set; } = new List<CardPhoto>();
}
