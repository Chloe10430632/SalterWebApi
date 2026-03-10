using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class HomMessage
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string? Content { get; set; }

    public DateTime? SendTime { get; set; }

    public bool? IsRead { get; set; }
}
