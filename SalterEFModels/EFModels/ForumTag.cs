using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumTag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<ForumPostTagDetail> ForumPostTagDetails { get; set; } = new List<ForumPostTagDetail>();
}
