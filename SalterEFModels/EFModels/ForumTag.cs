using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumTag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<ForumPost> Posts { get; set; } = new List<ForumPost>();
}
