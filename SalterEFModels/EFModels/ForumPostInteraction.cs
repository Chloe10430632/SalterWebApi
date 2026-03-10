using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumPostInteraction
{
    public int InteractionId { get; set; }

    public int PostId { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = null!;

    public string? ReportReason { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
