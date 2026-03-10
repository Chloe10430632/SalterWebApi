using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpEquipmentOrder
{
    public int Id { get; set; }

    public int? ExpEquipmentId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? UserId { get; set; }

    public int? ExpTransactionId { get; set; }

    public virtual ExpEquipment? ExpEquipment { get; set; }

    public virtual ExpTransaction? ExpTransaction { get; set; }

    public virtual UserUser? User { get; set; }
}
