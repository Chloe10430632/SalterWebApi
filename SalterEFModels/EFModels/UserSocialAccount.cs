using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserSocialAccount
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string Provider { get; set; } = null!;

    public string ProviderUserId { get; set; } = null!;

    public string AccessToken { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual UserUser User { get; set; } = null!;
}
