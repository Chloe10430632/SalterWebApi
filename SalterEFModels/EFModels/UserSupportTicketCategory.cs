using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserSupportTicketCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<UserFaqAnswer> UserFaqAnswers { get; set; } = new List<UserFaqAnswer>();

    public virtual ICollection<UserSupportTicket> UserSupportTickets { get; set; } = new List<UserSupportTicket>();
}
