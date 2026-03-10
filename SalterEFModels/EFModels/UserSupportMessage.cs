using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserSupportMessage
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string SenderType { get; set; } = null!;

    public int? AgentId { get; set; }

    public string Message { get; set; } = null!;

    public long? MatchedAnswerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual UserAdmin? Agent { get; set; }

    public virtual UserFaqAnswer? MatchedAnswer { get; set; }

    public virtual UserUser User { get; set; } = null!;
}
