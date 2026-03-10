using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserSupportTicket
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public int CategoryId { get; set; }

    public string Content { get; set; } = null!;

    public int? AssignedAgentId { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UserAdmin? AssignedAgent { get; set; }

    public virtual UserSupportTicketCategory Category { get; set; } = null!;

    public virtual UserStatus Status { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
