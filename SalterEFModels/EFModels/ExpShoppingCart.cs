using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpShoppingCart
{
    public int? ExpEquipmentId1 { get; set; }

    public int? Quantity1 { get; set; }

    public int? CourseSessionId { get; set; }

    public int? ExpEquipmentId2 { get; set; }

    public int? Quantity2 { get; set; }

    public int? ExpEquipmentId3 { get; set; }

    public int? Quantity3 { get; set; }

    public int? ExpEquipmentId4 { get; set; }

    public int? Quantity4 { get; set; }

    public int? UserId { get; set; }

    public virtual UserUser? User { get; set; }
}
