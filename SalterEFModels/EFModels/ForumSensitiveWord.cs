using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ForumSensitiveWord
{
    public int WordId { get; set; }

    public string Word { get; set; } = null!;

    public string Level { get; set; } = null!;
}
