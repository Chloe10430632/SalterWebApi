using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumComment
{
    public int CommentId { get; set; }

    public int PostId { get; set; }

    public int? ParentCommentId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ForumPost Post { get; set; } = null!;

    public virtual UserUser User { get; set; } = null!;
}
