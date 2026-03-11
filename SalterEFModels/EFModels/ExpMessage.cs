using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpMessage
{
    public int Id { get; set; }

    public int? SenderUserId { get; set; }

    public int? ReceiverCoachId { get; set; }

    public string? MessageContent { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual ExpCoach? ReceiverCoach { get; set; }

    public virtual UserUser? SenderUser { get; set; }
}
