using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardSportStat
{
    public int UserId { get; set; }

    public int TotalSurfingCount { get; set; }

    public decimal MaxWaveHeight { get; set; }

    public int TotalDivingCount { get; set; }

    public decimal TotalSwimmingDistanceKm { get; set; }

    public int TotalActivityTimeHours { get; set; }

    public DateTime LastUpdated { get; set; }
}
