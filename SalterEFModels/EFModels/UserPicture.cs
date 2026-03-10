using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserPicture
{
    public long Id { get; set; }

    public byte[] Picture { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<UserAdmin> UserAdmins { get; set; } = new List<UserAdmin>();
}
