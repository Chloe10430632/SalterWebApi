using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardActivityType
{
    public int ActivityTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<CardSocialActivity> CardSocialActivities { get; set; } = new List<CardSocialActivity>();
}
