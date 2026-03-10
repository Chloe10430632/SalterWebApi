using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumBoardInteraction
{
    public int InteractionId { get; set; }

    public int BoardId { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ForumBoardCategory Board { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
