using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumPostsImage
{
    public int ImageId { get; set; }

    public int PostId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int SortIndex { get; set; }

    public virtual ForumPost Post { get; set; } = null!;
}
