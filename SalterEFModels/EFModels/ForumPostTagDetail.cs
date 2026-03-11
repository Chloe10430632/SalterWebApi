using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumPostTagDetail
{
    public int Id { get; set; }

    public int PostId { get; set; }

    public int TagId { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual ForumTag Tag { get; set; } = null!;
}
