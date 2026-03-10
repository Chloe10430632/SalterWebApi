using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardMonitorSession
{
    public int MonitorSessionId { get; set; }

    public int UserId { get; set; }

    public int CoastalLocationId { get; set; }

    public int? ActivityTypeId { get; set; }

    public DateOnly? ActivityDate { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<CardMonitorRecord> CardMonitorRecords { get; set; } = new List<CardMonitorRecord>();

    public virtual CardCoastLocation CoastalLocation { get; set; } = null!;
}
