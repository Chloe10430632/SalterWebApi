using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumBoardCategory
{
    public int BoardId { get; set; }

    public string? ImageUrl { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public virtual ICollection<ForumBoardInteraction> ForumBoardInteractions { get; set; } = new List<ForumBoardInteraction>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
}
