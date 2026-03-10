using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserAdmin
{
    public int Id { get; set; }

    public string AdminName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int AdminRoleId { get; set; }

    public int StatusId { get; set; }

    public long? PictureId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UserAdminRole AdminRole { get; set; } = null!;

    public virtual ICollection<ExpEquipment> ExpEquipments { get; set; } = new List<ExpEquipment>();

    public virtual ICollection<ExpReport> ExpReports { get; set; } = new List<ExpReport>();

    public virtual UserPicture? Picture { get; set; }

    public virtual UserStatus Status { get; set; } = null!;

    public virtual ICollection<UserAccountDeletion> UserAccountDeletions { get; set; } = new List<UserAccountDeletion>();

    public virtual ICollection<UserSupportMessage> UserSupportMessages { get; set; } = new List<UserSupportMessage>();

    public virtual ICollection<UserSupportTicket> UserSupportTickets { get; set; } = new List<UserSupportTicket>();
}
