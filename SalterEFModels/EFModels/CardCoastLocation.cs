using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardCoastLocation
{
    public int CoastalLocationId { get; set; }

    public string CoastalName { get; set; } = null!;

    public string? CountyName { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longtitude { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<CardMonitorRecord> CardMonitorRecords { get; set; } = new List<CardMonitorRecord>();

    public virtual ICollection<CardMonitorSession> CardMonitorSessions { get; set; } = new List<CardMonitorSession>();
}
