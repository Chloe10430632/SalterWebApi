using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardPhoto
{
    public int PhotoId { get; set; }

    public int AlbumId { get; set; }

    public int? MonitorRecordId { get; set; }

    public string? Photo { get; set; }

    public string? PhotoDescription { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual CardAlbum Album { get; set; } = null!;

    public virtual CardMonitorRecord? MonitorRecord { get; set; }
}
