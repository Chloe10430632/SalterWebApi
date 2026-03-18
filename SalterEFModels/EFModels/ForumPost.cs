using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumPost
{
    public int PostId { get; set; }

    public int UserId { get; set; }

    public int BoardId { get; set; }

    public string Content { get; set; } = null!;

    public int? LocationId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsPinned { get; set; }

    public bool IsPosted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ForumBoardCategory Board { get; set; } = null!;

    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    public virtual ICollection<ForumPostInteraction> ForumPostInteractions { get; set; } = new List<ForumPostInteraction>();

    public virtual ICollection<ForumPostTagDetail> ForumPostTagDetails { get; set; } = new List<ForumPostTagDetail>();

    public virtual ICollection<ForumPostsImage> ForumPostsImages { get; set; } = new List<ForumPostsImage>();

    public virtual TripLocation? Location { get; set; }

    public virtual UserUser User { get; set; } = null!;
}
