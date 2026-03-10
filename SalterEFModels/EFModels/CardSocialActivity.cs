using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardSocialActivity
{
    public int CardActivityId { get; set; }

    public int? MonitorRecordId { get; set; }

    public int? ShowRecordId { get; set; }

    public int? UserId { get; set; }

    public int? ActivityTypeId { get; set; }

    public string? CardActivityTitle { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual CardActivityType? ActivityType { get; set; }
}
