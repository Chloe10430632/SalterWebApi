using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserFaqAnswer
{
    public long Id { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public int? CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UserSupportTicketCategory? Category { get; set; }

    public virtual ICollection<UserSupportMessage> UserSupportMessages { get; set; } = new List<UserSupportMessage>();
}
