using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserAccountDeletion
{
    public long Id { get; set; }

    public int AccountId { get; set; }

    public int? RequestedByUserId { get; set; }

    public int? RequestedByAdminId { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual UserUser Account { get; set; } = null!;

    public virtual UserAdmin? RequestedByAdmin { get; set; }

    public virtual UserUser? RequestedByUser { get; set; }
}
