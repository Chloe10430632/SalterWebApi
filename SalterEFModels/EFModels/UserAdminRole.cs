using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserAdminRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserAdmin> UserAdmins { get; set; } = new List<UserAdmin>();
}
