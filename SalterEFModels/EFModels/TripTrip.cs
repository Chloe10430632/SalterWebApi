using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class TripTrip
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string TripType { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? LockedAt { get; set; }

    public int OrganizerUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? CoverImagePublicId { get; set; }

    public virtual UserUser OrganizerUser { get; set; } = null!;

    public virtual ICollection<TripAnnouncement> TripAnnouncements { get; set; } = new List<TripAnnouncement>();

    public virtual ICollection<TripFavorite> TripFavorites { get; set; } = new List<TripFavorite>();

    public virtual ICollection<TripGearItem> TripGearItems { get; set; } = new List<TripGearItem>();

    public virtual ICollection<TripMember> TripMembers { get; set; } = new List<TripMember>();

    public virtual ICollection<TripNote> TripNotes { get; set; } = new List<TripNote>();

    public virtual ICollection<TripReminder> TripReminders { get; set; } = new List<TripReminder>();

    public virtual ICollection<TripTimeline> TripTimelines { get; set; } = new List<TripTimeline>();

    public virtual ICollection<TripTripLocation> TripTripLocations { get; set; } = new List<TripTripLocation>();
}
