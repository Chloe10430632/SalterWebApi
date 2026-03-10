using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardMonitorRecord
{
    public int MonitorRecordId { get; set; }

    public int MonitorSessionId { get; set; }

    public int? UserId { get; set; }

    public int? CoastalLocationId { get; set; }

    public decimal? FeelsLikeTemperature { get; set; }

    public decimal? SeaTemperature { get; set; }

    public decimal? AirTemperature { get; set; }

    public decimal? MaxWaveHeight { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CardPhoto> CardPhotos { get; set; } = new List<CardPhoto>();

    public virtual CardCoastLocation? CoastalLocation { get; set; }

    public virtual CardMonitorSession MonitorSession { get; set; } = null!;
}
