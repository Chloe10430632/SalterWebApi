using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardProfile
{
    public int CardProfileId { get; set; }

    public int UserId { get; set; }

    public string? AvatarPicture { get; set; }

    public bool IsCardPublic { get; set; }

    public bool AllowFriendRequest { get; set; }

    public bool AllowStrangerMessage { get; set; }

    public string? CardMemo { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<CardChatRoom> CardChatRoomUser1s { get; set; } = new List<CardChatRoom>();

    public virtual ICollection<CardChatRoom> CardChatRoomUser2s { get; set; } = new List<CardChatRoom>();

    public virtual ICollection<CardFriendship> CardFriendships { get; set; } = new List<CardFriendship>();
}
