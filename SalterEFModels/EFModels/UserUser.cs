using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class UserUser
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int UserRoleId { get; set; }

    public int StatusId { get; set; }

    public string? Phone { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? ProfilePicture { get; set; }

    public bool IsActive { get; set; }

    public string? EmailVerificationOtp { get; set; }

    public DateTime? EmailVerificationExpiresAt { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public string? PasswordResetOtp { get; set; }

    public DateTime? PasswordResetExpiresAt { get; set; }

    public DateTime? PasswordChangedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ExpCoach> ExpCoaches { get; set; } = new List<ExpCoach>();

    public virtual ICollection<ExpCourseOrder> ExpCourseOrders { get; set; } = new List<ExpCourseOrder>();

    public virtual ICollection<ExpEquipmentOrder> ExpEquipmentOrders { get; set; } = new List<ExpEquipmentOrder>();

    public virtual ICollection<ExpFavorite> ExpFavorites { get; set; } = new List<ExpFavorite>();

    public virtual ICollection<ExpMessage> ExpMessages { get; set; } = new List<ExpMessage>();

    public virtual ICollection<ExpReview> ExpReviews { get; set; } = new List<ExpReview>();

    public virtual ICollection<ExpTransaction> ExpTransactionReceiveUsers { get; set; } = new List<ExpTransaction>();

    public virtual ICollection<ExpTransaction> ExpTransactionSenderUsers { get; set; } = new List<ExpTransaction>();

    public virtual ICollection<ForumAd> ForumAds { get; set; } = new List<ForumAd>();

    public virtual ICollection<ForumBoardInteraction> ForumBoardInteractions { get; set; } = new List<ForumBoardInteraction>();

    public virtual ICollection<ForumComment> ForumComments { get; set; } = new List<ForumComment>();

    public virtual ICollection<ForumPostInteraction> ForumPostInteractions { get; set; } = new List<ForumPostInteraction>();

    public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();

    public virtual ICollection<HomBooking> HomBookings { get; set; } = new List<HomBooking>();

    public virtual UserStatus Status { get; set; } = null!;

    public virtual ICollection<TripAnnouncement> TripAnnouncements { get; set; } = new List<TripAnnouncement>();

    public virtual ICollection<TripFavoriteFolder> TripFavoriteFolders { get; set; } = new List<TripFavoriteFolder>();

    public virtual ICollection<TripFavorite> TripFavorites { get; set; } = new List<TripFavorite>();

    public virtual ICollection<TripGearCheck> TripGearChecks { get; set; } = new List<TripGearCheck>();

    public virtual ICollection<TripGearItem> TripGearItems { get; set; } = new List<TripGearItem>();

    public virtual ICollection<TripMember> TripMembers { get; set; } = new List<TripMember>();

    public virtual ICollection<TripTrip> TripTrips { get; set; } = new List<TripTrip>();

    public virtual ICollection<UserAccountDeletion> UserAccountDeletionAccounts { get; set; } = new List<UserAccountDeletion>();

    public virtual ICollection<UserAccountDeletion> UserAccountDeletionRequestedByUsers { get; set; } = new List<UserAccountDeletion>();

    public virtual UserUserRole UserRole { get; set; } = null!;

    public virtual ICollection<UserSocialAccount> UserSocialAccounts { get; set; } = new List<UserSocialAccount>();

    public virtual ICollection<UserSupportMessage> UserSupportMessages { get; set; } = new List<UserSupportMessage>();

    public virtual ICollection<UserSupportTicket> UserSupportTickets { get; set; } = new List<UserSupportTicket>();
}
