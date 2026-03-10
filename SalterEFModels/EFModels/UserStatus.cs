using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserAdmin> UserAdmins { get; set; } = new List<UserAdmin>();

    public virtual ICollection<UserSupportTicket> UserSupportTickets { get; set; } = new List<UserSupportTicket>();

    public virtual ICollection<UserUser> UserUsers { get; set; } = new List<UserUser>();
}
