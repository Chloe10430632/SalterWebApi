using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardChatRoom
{
    public int ChatRoomId { get; set; }

    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public DateTime LastMessageAt { get; set; }

    public virtual ICollection<CardChatMessage> CardChatMessages { get; set; } = new List<CardChatMessage>();

    public virtual CardProfile User1 { get; set; } = null!;

    public virtual CardProfile User2 { get; set; } = null!;
}
