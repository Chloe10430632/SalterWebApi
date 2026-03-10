using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserUserRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserUser> UserUsers { get; set; } = new List<UserUser>();
}
