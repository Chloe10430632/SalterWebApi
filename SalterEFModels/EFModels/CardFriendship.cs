using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardFriendship
{
    public int FriendshipId { get; set; }

    public int UserId { get; set; }

    public int FriendId { get; set; }

    public int Status { get; set; }

    public virtual CardProfile User { get; set; } = null!;
}
