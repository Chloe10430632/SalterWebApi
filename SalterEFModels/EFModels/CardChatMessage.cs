using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class CardChatMessage
{
    public int MessageId { get; set; }

    public int ChatRoomId { get; set; }

    public int SenderId { get; set; }

    public string? MessageText { get; set; }

    public bool IsRead { get; set; }

    public DateTime SentAt { get; set; }

    public virtual CardChatRoom ChatRoom { get; set; } = null!;
}
