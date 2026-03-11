using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SalterEFModels.EFModels;

public partial class SalterDbContext : DbContext
{
    public SalterDbContext()
    {
    }

    public SalterDbContext(DbContextOptions<SalterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CardActivityType> CardActivityTypes { get; set; }

    public virtual DbSet<CardAlbum> CardAlbums { get; set; }

    public virtual DbSet<CardChatMessage> CardChatMessages { get; set; }

    public virtual DbSet<CardChatRoom> CardChatRooms { get; set; }

    public virtual DbSet<CardCoastLocation> CardCoastLocations { get; set; }

    public virtual DbSet<CardFriendship> CardFriendships { get; set; }

    public virtual DbSet<CardMonitorRecord> CardMonitorRecords { get; set; }

    public virtual DbSet<CardMonitorSession> CardMonitorSessions { get; set; }

    public virtual DbSet<CardPhoto> CardPhotos { get; set; }

    public virtual DbSet<CardProfile> CardProfiles { get; set; }

    public virtual DbSet<CardSocialActivity> CardSocialActivities { get; set; }

    public virtual DbSet<CardSportStat> CardSportStats { get; set; }

    public virtual DbSet<ExpCoach> ExpCoaches { get; set; }

    public virtual DbSet<ExpCourseOrder> ExpCourseOrders { get; set; }

    public virtual DbSet<ExpCoursePhoto> ExpCoursePhotos { get; set; }

    public virtual DbSet<ExpCourseSession> ExpCourseSessions { get; set; }

    public virtual DbSet<ExpCourseTemplate> ExpCourseTemplates { get; set; }

    public virtual DbSet<ExpEquipment> ExpEquipments { get; set; }

    public virtual DbSet<ExpEquipmentOrder> ExpEquipmentOrders { get; set; }

    public virtual DbSet<ExpEquipmentPicture> ExpEquipmentPictures { get; set; }

    public virtual DbSet<ExpFavorite> ExpFavorites { get; set; }

    public virtual DbSet<ExpMessage> ExpMessages { get; set; }

    public virtual DbSet<ExpReport> ExpReports { get; set; }

    public virtual DbSet<ExpReview> ExpReviews { get; set; }

    public virtual DbSet<ExpShoppingCart> ExpShoppingCarts { get; set; }

    public virtual DbSet<ExpSpeciality> ExpSpecialities { get; set; }

    public virtual DbSet<ExpTransaction> ExpTransactions { get; set; }

    public virtual DbSet<ExpTransactionsType> ExpTransactionsTypes { get; set; }

    public virtual DbSet<ExpTthirdPartyPayment> ExpTthirdPartyPayments { get; set; }

    public virtual DbSet<ForumAd> ForumAds { get; set; }

    public virtual DbSet<ForumBoardCategory> ForumBoardCategories { get; set; }

    public virtual DbSet<ForumBoardInteraction> ForumBoardInteractions { get; set; }

    public virtual DbSet<ForumComment> ForumComments { get; set; }

    public virtual DbSet<ForumPost> ForumPosts { get; set; }

    public virtual DbSet<ForumPostInteraction> ForumPostInteractions { get; set; }

    public virtual DbSet<ForumPostTagDetail> ForumPostTagDetails { get; set; }

    public virtual DbSet<ForumPostsImage> ForumPostsImages { get; set; }

    public virtual DbSet<ForumSensitiveWord> ForumSensitiveWords { get; set; }

    public virtual DbSet<ForumTag> ForumTags { get; set; }

    public virtual DbSet<HomAmenity> HomAmenities { get; set; }

    public virtual DbSet<HomBooking> HomBookings { get; set; }

    public virtual DbSet<HomCitieMapping> HomCitieMappings { get; set; }

    public virtual DbSet<HomDistrictMapping> HomDistrictMappings { get; set; }

    public virtual DbSet<HomHouse> HomHouses { get; set; }

    public virtual DbSet<HomLocationMapping> HomLocationMappings { get; set; }

    public virtual DbSet<HomMessage> HomMessages { get; set; }

    public virtual DbSet<HomReview> HomReviews { get; set; }

    public virtual DbSet<HomRoomCalendar> HomRoomCalendars { get; set; }

    public virtual DbSet<HomRoomImage> HomRoomImages { get; set; }

    public virtual DbSet<HomRoomType> HomRoomTypes { get; set; }

    public virtual DbSet<HomRoomTypeAmenity> HomRoomTypeAmenities { get; set; }

    public virtual DbSet<TripAnnouncement> TripAnnouncements { get; set; }

    public virtual DbSet<TripCity> TripCities { get; set; }

    public virtual DbSet<TripDistrict> TripDistricts { get; set; }

    public virtual DbSet<TripFavorite> TripFavorites { get; set; }

    public virtual DbSet<TripGearCheck> TripGearChecks { get; set; }

    public virtual DbSet<TripGearItem> TripGearItems { get; set; }

    public virtual DbSet<TripLocation> TripLocations { get; set; }

    public virtual DbSet<TripMember> TripMembers { get; set; }

    public virtual DbSet<TripNote> TripNotes { get; set; }

    public virtual DbSet<TripReminder> TripReminders { get; set; }

    public virtual DbSet<TripTimeline> TripTimelines { get; set; }

    public virtual DbSet<TripTrip> TripTrips { get; set; }

    public virtual DbSet<TripTripLocation> TripTripLocations { get; set; }

    public virtual DbSet<UserAccountDeletion> UserAccountDeletions { get; set; }

    public virtual DbSet<UserAdmin> UserAdmins { get; set; }

    public virtual DbSet<UserAdminRole> UserAdminRoles { get; set; }

    public virtual DbSet<UserFaqAnswer> UserFaqAnswers { get; set; }

    public virtual DbSet<UserPicture> UserPictures { get; set; }

    public virtual DbSet<UserSocialAccount> UserSocialAccounts { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    public virtual DbSet<UserSupportMessage> UserSupportMessages { get; set; }

    public virtual DbSet<UserSupportTicket> UserSupportTickets { get; set; }

    public virtual DbSet<UserSupportTicketCategory> UserSupportTicketCategories { get; set; }

    public virtual DbSet<UserUser> UserUsers { get; set; }

    public virtual DbSet<UserUserRole> UserUserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=123.192.123.82,1433;Initial Catalog=Salter;User ID=sa;Password=DX9Qu!3wKWXrbyk;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CardActivityType>(entity =>
        {
            entity.HasKey(e => e.ActivityTypeId).HasName("PK__CardActi__3893A5837F59F4C8");

            entity.ToTable("CardActivityType");

            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_TypeID");
            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .HasColumnName("type_Name");
        });

        modelBuilder.Entity<CardAlbum>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK__CardAlbu__75BF3EEFBDAD1394");

            entity.ToTable("CardAlbum");

            entity.Property(e => e.AlbumId).HasColumnName("albumID");
            entity.Property(e => e.AlbumName)
                .HasMaxLength(50)
                .HasColumnName("album_Name");
            entity.Property(e => e.CoverPhoto)
                .HasMaxLength(255)
                .HasColumnName("cover_Photo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsEnable)
                .HasDefaultValue(true)
                .HasColumnName("isEnable");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<CardChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__CardChat__4808B87375C579A5");

            entity.ToTable("CardChatMessage");

            entity.Property(e => e.MessageId).HasColumnName("messageID");
            entity.Property(e => e.ChatRoomId).HasColumnName("chatRoomID");
            entity.Property(e => e.IsRead).HasColumnName("isRead");
            entity.Property(e => e.MessageText).HasColumnName("message_Text");
            entity.Property(e => e.SenderId).HasColumnName("senderID");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sentAt");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.CardChatMessages)
                .HasForeignKey(d => d.ChatRoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatMessage_ChatRoom1");
        });

        modelBuilder.Entity<CardChatRoom>(entity =>
        {
            entity.HasKey(e => e.ChatRoomId).HasName("PK__CardChat__CB58B472339D0BFA");

            entity.ToTable("CardChatRoom");

            entity.Property(e => e.ChatRoomId).HasColumnName("chatRoomID");
            entity.Property(e => e.LastMessageAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_MessageAt");
            entity.Property(e => e.User1Id).HasColumnName("user1ID");
            entity.Property(e => e.User2Id).HasColumnName("user2ID");

            entity.HasOne(d => d.User1).WithMany(p => p.CardChatRoomUser1s)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.User1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatRoom_CardProfile");

            entity.HasOne(d => d.User2).WithMany(p => p.CardChatRoomUser2s)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.User2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChatRoom_CardProfile1");
        });

        modelBuilder.Entity<CardCoastLocation>(entity =>
        {
            entity.HasKey(e => e.CoastalLocationId).HasName("PK_CoastalLocation");

            entity.ToTable("CardCoastLocation");

            entity.Property(e => e.CoastalLocationId)
                .ValueGeneratedNever()
                .HasColumnName("coastal_location_id");
            entity.Property(e => e.CoastalName)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("coastal_name");
            entity.Property(e => e.CountyName)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("county_name");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longtitude)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("longtitude");
        });

        modelBuilder.Entity<CardFriendship>(entity =>
        {
            entity.HasKey(e => e.FriendshipId).HasName("PK__CardFrie__1E39AD123DAB5D1B");

            entity.ToTable("CardFriendship");

            entity.Property(e => e.FriendshipId).HasColumnName("friendshipID");
            entity.Property(e => e.FriendId).HasColumnName("friendID");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.CardFriendships)
                .HasPrincipalKey(p => p.UserId)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_CardProfile");
        });

        modelBuilder.Entity<CardMonitorRecord>(entity =>
        {
            entity.HasKey(e => e.MonitorRecordId).HasName("PK__CardMoni__8A0480DA4C4B88BC");

            entity.ToTable("CardMonitorRecord");

            entity.Property(e => e.MonitorRecordId).HasColumnName("monitor_RecordID");
            entity.Property(e => e.AirTemperature)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("air_Temperature");
            entity.Property(e => e.CoastalLocationId).HasColumnName("coastal_LocationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.FeelsLikeTemperature)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("FeelsLike_Temperature");
            entity.Property(e => e.MaxWaveHeight)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("max_WaveHeight");
            entity.Property(e => e.MonitorSessionId).HasColumnName("monitor_SessionID");
            entity.Property(e => e.SeaTemperature)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("sea_Temperature");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.CoastalLocation).WithMany(p => p.CardMonitorRecords)
                .HasForeignKey(d => d.CoastalLocationId)
                .HasConstraintName("FK_MonitorRecord_Coastal");

            entity.HasOne(d => d.MonitorSession).WithMany(p => p.CardMonitorRecords)
                .HasForeignKey(d => d.MonitorSessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonitorRecord_MonitorSession");
        });

        modelBuilder.Entity<CardMonitorSession>(entity =>
        {
            entity.HasKey(e => e.MonitorSessionId).HasName("PK__CardMoni__06475CC414CCE81F");

            entity.ToTable("CardMonitorSession");

            entity.Property(e => e.MonitorSessionId).HasColumnName("monitor_SessionID");
            entity.Property(e => e.ActivityDate).HasColumnName("activity_Date");
            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_TypeID");
            entity.Property(e => e.CoastalLocationId).HasColumnName("coastal_LocationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.CoastalLocation).WithMany(p => p.CardMonitorSessions)
                .HasForeignKey(d => d.CoastalLocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonitorSession_Coastal");
        });

        modelBuilder.Entity<CardPhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__CardPhot__547C32CD25D9D587");

            entity.ToTable("CardPhoto");

            entity.Property(e => e.PhotoId).HasColumnName("photoID");
            entity.Property(e => e.AlbumId).HasColumnName("albumID");
            entity.Property(e => e.MonitorRecordId).HasColumnName("monitor_RecordID");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
            entity.Property(e => e.PhotoDescription).HasColumnName("photo_Description");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("uploadedAt");

            entity.HasOne(d => d.Album).WithMany(p => p.CardPhotos)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CardPhoto_Album");

            entity.HasOne(d => d.MonitorRecord).WithMany(p => p.CardPhotos)
                .HasForeignKey(d => d.MonitorRecordId)
                .HasConstraintName("FK_CardPhoto_Record");
        });

        modelBuilder.Entity<CardProfile>(entity =>
        {
            entity.HasKey(e => e.CardProfileId).HasName("PK__CardProf__F787936DBA1FDA93");

            entity.ToTable("CardProfile");

            entity.HasIndex(e => e.UserId, "UQ__CardProf__CB9A1CDEB7751B77").IsUnique();

            entity.Property(e => e.CardProfileId).HasColumnName("card_ProfileID");
            entity.Property(e => e.AllowFriendRequest)
                .HasDefaultValue(true)
                .HasColumnName("allow_Friend_Request");
            entity.Property(e => e.AllowStrangerMessage)
                .HasDefaultValue(true)
                .HasColumnName("allow_Stranger_Message");
            entity.Property(e => e.AvatarPicture)
                .HasMaxLength(255)
                .HasColumnName("avatar_picture");
            entity.Property(e => e.CardMemo)
                .HasMaxLength(255)
                .HasColumnName("card_Memo");
            entity.Property(e => e.IsCardPublic)
                .HasDefaultValue(true)
                .HasColumnName("is_Card_Public");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<CardSocialActivity>(entity =>
        {
            entity.HasKey(e => e.CardActivityId).HasName("PK__CardSoci__C27540E4C4DD4A1A");

            entity.ToTable("CardSocialActivity");

            entity.Property(e => e.CardActivityId).HasColumnName("card_ActivityID");
            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_TypeID");
            entity.Property(e => e.CardActivityTitle)
                .HasMaxLength(100)
                .HasColumnName("card_Activity_title");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(true)
                .HasColumnName("isPublic");
            entity.Property(e => e.MonitorRecordId).HasColumnName("monitor_RecordID");
            entity.Property(e => e.ShowRecordId).HasColumnName("show_RecordID");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.ActivityType).WithMany(p => p.CardSocialActivities)
                .HasForeignKey(d => d.ActivityTypeId)
                .HasConstraintName("FK_CardActivity_ActivityType");
        });

        modelBuilder.Entity<CardSportStat>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__CardSpor__CB9A1CDF6F8C40E3");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("userID");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_Updated");
            entity.Property(e => e.MaxWaveHeight)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("max_Wave_Height");
            entity.Property(e => e.TotalActivityTimeHours).HasColumnName("total_Activity_Time_Hours");
            entity.Property(e => e.TotalDivingCount).HasColumnName("total_Diving_Count");
            entity.Property(e => e.TotalSurfingCount).HasColumnName("total_Surfing_Count");
            entity.Property(e => e.TotalSwimmingDistanceKm)
                .HasColumnType("decimal(7, 2)")
                .HasColumnName("total_Swimming_Distance_km");
        });

        modelBuilder.Entity<ExpCoach>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__expCoach__3213E83F9CCD4413");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatar_url");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Introduction)
                .HasMaxLength(500)
                .HasColumnName("introduction");
            entity.Property(e => e.IsSuspended).HasColumnName("is_suspended");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.City).WithMany(p => p.ExpCoaches)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_ExpCoaches_TripCities");

            entity.HasOne(d => d.District).WithMany(p => p.ExpCoaches)
                .HasForeignKey(d => d.DistrictId)
                .HasConstraintName("FK_ExpCoaches_TripDistricts");

            entity.HasOne(d => d.User).WithMany(p => p.ExpCoaches)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ExpCoaches_UserUsers");

            entity.HasMany(d => d.Specialities).WithMany(p => p.Coaches)
                .UsingEntity<Dictionary<string, object>>(
                    "ExpCoachSpeciallityMapping",
                    r => r.HasOne<ExpSpeciality>().WithMany()
                        .HasForeignKey("SpecialitiesId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_expCoachSpeciallityMapping_ExpSpecialities"),
                    l => l.HasOne<ExpCoach>().WithMany()
                        .HasForeignKey("CoachId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_expCoachSpeciallityMapping_ExpCoaches"),
                    j =>
                    {
                        j.HasKey("CoachId", "SpecialitiesId");
                        j.ToTable("ExpCoachSpeciallityMapping");
                        j.IndexerProperty<int>("CoachId").HasColumnName("coach_id");
                        j.IndexerProperty<int>("SpecialitiesId").HasColumnName("specialities_id");
                    });

            entity.HasMany(d => d.TripDistricts).WithMany(p => p.CoachDists)
                .UsingEntity<Dictionary<string, object>>(
                    "ExpDistrictMapping",
                    r => r.HasOne<TripDistrict>().WithMany()
                        .HasForeignKey("TripDistrictId")
                        .HasConstraintName("FK_ExpDistrictMapping_TripDistrict"),
                    l => l.HasOne<ExpCoach>().WithMany()
                        .HasForeignKey("CoachDistId")
                        .HasConstraintName("FK_ExpDistrictMapping_ExpCoach"),
                    j =>
                    {
                        j.HasKey("CoachDistId", "TripDistrictId");
                        j.ToTable("ExpDistrictMapping");
                        j.IndexerProperty<int>("CoachDistId").HasColumnName("coach_dist_id");
                        j.IndexerProperty<int>("TripDistrictId").HasColumnName("trip_district_id");
                    });
        });

        modelBuilder.Entity<ExpCourseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpCours__3213E83F192872B6");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CourseSessionId).HasColumnName("course_session_id");
            entity.Property(e => e.ExpTransactionId).HasColumnName("expTransaction_id");
            entity.Property(e => e.ReservedAt).HasColumnName("reserved_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.CourseSession).WithMany(p => p.ExpCourseOrders)
                .HasForeignKey(d => d.CourseSessionId)
                .HasConstraintName("FK__ExpCourse__cours__1D7B6025");

            entity.HasOne(d => d.ExpTransaction).WithMany(p => p.ExpCourseOrders)
                .HasForeignKey(d => d.ExpTransactionId)
                .HasConstraintName("FK__ExpCourse__expTr__1E6F845E");

            entity.HasOne(d => d.User).WithMany(p => p.ExpCourseOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_expCourseOrders_UserUsers");
        });

        modelBuilder.Entity<ExpCoursePhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpCours__3213E83F882674AC");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CourseTemplateId).HasColumnName("course_template_id");
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
            entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at");

            entity.HasOne(d => d.CourseTemplate).WithMany(p => p.ExpCoursePhotos)
                .HasForeignKey(d => d.CourseTemplateId)
                .HasConstraintName("FK__ExpCourse__cours__2057CCD0");
        });

        modelBuilder.Entity<ExpCourseSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpCours__3213E83FEE318725");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CoachId).HasColumnName("coach_id");
            entity.Property(e => e.CourseTemplateId).HasColumnName("course_template_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CurrentParticipants).HasColumnName("current_participants");
            entity.Property(e => e.MaxParticipants).HasColumnName("max_participants");
            entity.Property(e => e.SessionDate).HasColumnName("session_date");
            entity.Property(e => e.TimeSlot)
                .HasMaxLength(50)
                .HasColumnName("time_slot");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.CourseTemplate).WithMany(p => p.ExpCourseSessions)
                .HasForeignKey(d => d.CourseTemplateId)
                .HasConstraintName("FK__ExpCourse__cours__214BF109");
        });

        modelBuilder.Entity<ExpCourseTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpCours__3213E83F4ACBE606");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CoachId).HasColumnName("coach_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(50)
                .HasColumnName("difficulty");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .HasColumnName("location");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(7, 0)")
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Coach).WithMany(p => p.ExpCourseTemplates)
                .HasForeignKey(d => d.CoachId)
                .HasConstraintName("FK_expCourseTemplates_ExpCoaches");

            entity.HasOne(d => d.LocationNavigation).WithMany(p => p.ExpCourseTemplates)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_expCourseTemplates_TripLocations");
        });

        modelBuilder.Entity<ExpEquipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__expEquip__3213E83F5C5F64EA");

            entity.ToTable("ExpEquipment");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CoachId).HasColumnName("coach_id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.IsRental).HasColumnName("is_rental");
            entity.Property(e => e.ListedAt).HasColumnName("listed_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(7, 0)")
                .HasColumnName("price");
            entity.Property(e => e.ReviewAdminId).HasColumnName("review_admin_id");
            entity.Property(e => e.ReviewRemark)
                .HasMaxLength(500)
                .HasColumnName("review_remark");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Coach).WithMany(p => p.ExpEquipments)
                .HasForeignKey(d => d.CoachId)
                .HasConstraintName("FK_expEquipment_expCoaches");

            entity.HasOne(d => d.ReviewAdmin).WithMany(p => p.ExpEquipments)
                .HasForeignKey(d => d.ReviewAdminId)
                .HasConstraintName("FK_expEquipment_UserAdmins");
        });

        modelBuilder.Entity<ExpEquipmentOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpEquip__3213E83FC30DC748");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ExpEquipmentId).HasColumnName("expEquipment_id");
            entity.Property(e => e.ExpTransactionId).HasColumnName("expTransaction_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(7, 0)")
                .HasColumnName("unit_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ExpEquipment).WithMany(p => p.ExpEquipmentOrders)
                .HasForeignKey(d => d.ExpEquipmentId)
                .HasConstraintName("FK__expEquipm__expEq__39AD8A7F");

            entity.HasOne(d => d.ExpTransaction).WithMany(p => p.ExpEquipmentOrders)
                .HasForeignKey(d => d.ExpTransactionId)
                .HasConstraintName("FK__ExpEquipm__expTr__27F8EE98");

            entity.HasOne(d => d.User).WithMany(p => p.ExpEquipmentOrders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_expEquipmentOrders_UserUsers");
        });

        modelBuilder.Entity<ExpEquipmentPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpEquip__3213E83FFEA2E948");

            entity.ToTable("ExpEquipmentPicture");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EPhotoUrl).HasColumnName("e_photo_url");
            entity.Property(e => e.ExpEquipmentId).HasColumnName("expEquipment_id");
            entity.Property(e => e.UploadedAt).HasColumnName("uploaded_at");

            entity.HasOne(d => d.ExpEquipment).WithMany(p => p.ExpEquipmentPictures)
                .HasForeignKey(d => d.ExpEquipmentId)
                .HasConstraintName("FK__expEquipm__expEq__37C5420D");
        });

        modelBuilder.Entity<ExpFavorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ExpFavorites_1");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CoachId).HasColumnName("coach_id");
            entity.Property(e => e.FavoritedAt).HasColumnName("favorited_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Coach).WithMany(p => p.ExpFavorites)
                .HasForeignKey(d => d.CoachId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_expFavorites_expCoaches");

            entity.HasOne(d => d.User).WithMany(p => p.ExpFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_expFavorites_UserUsers");
        });

        modelBuilder.Entity<ExpMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpMessa__3213E83FFC1A4F93");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.MessageContent).HasColumnName("message_content");
            entity.Property(e => e.ReceiverCoachId).HasColumnName("receiver_coach_id");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.SentAt).HasColumnName("sent_at");

            entity.HasOne(d => d.ReceiverCoach).WithMany(p => p.ExpMessages)
                .HasForeignKey(d => d.ReceiverCoachId)
                .HasConstraintName("FK_ExpMessages_ExpCoaches");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.ExpMessages)
                .HasForeignKey(d => d.SenderUserId)
                .HasConstraintName("FK_ExpMessages_UserUsers");
        });

        modelBuilder.Entity<ExpReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpRepor__3213E83F0B1DE74A");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.HandledByAdminId).HasColumnName("handled_by_admin_id");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TargetCoachId).HasColumnName("target_coach_id");
            entity.Property(e => e.TargetReviewId).HasColumnName("target_review_id");

            entity.HasOne(d => d.HandledByAdmin).WithMany(p => p.ExpReports)
                .HasForeignKey(d => d.HandledByAdminId)
                .HasConstraintName("FK_expReports_UserAdmins");

            entity.HasOne(d => d.TargetReview).WithMany(p => p.ExpReports)
                .HasForeignKey(d => d.TargetReviewId)
                .HasConstraintName("FK_expReports_expReviews");
        });

        modelBuilder.Entity<ExpReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpRevie__3213E83F70B6B9A2");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CoachId).HasColumnName("coach_id");
            entity.Property(e => e.CourseOrderId).HasColumnName("course_order_id");
            entity.Property(e => e.IsHidden).HasColumnName("is_hidden");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewContent).HasColumnName("review_content");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Coach).WithMany(p => p.ExpReviews)
                .HasForeignKey(d => d.CoachId)
                .HasConstraintName("FK_ExpReviews_ExpCoaches");

            entity.HasOne(d => d.CourseOrder).WithMany(p => p.ExpReviews)
                .HasForeignKey(d => d.CourseOrderId)
                .HasConstraintName("FK__ExpReview__cours__308E3499");

            entity.HasOne(d => d.User).WithMany(p => p.ExpReviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_expReviews_UserUsers");
        });

        modelBuilder.Entity<ExpShoppingCart>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExpShoppingCart");

            entity.Property(e => e.CourseSessionId).HasColumnName("courseSession_id");
            entity.Property(e => e.ExpEquipmentId1).HasColumnName("ExpEquipment_id1");
            entity.Property(e => e.ExpEquipmentId2).HasColumnName("ExpEquipment_id2");
            entity.Property(e => e.ExpEquipmentId3).HasColumnName("ExpEquipment_id3");
            entity.Property(e => e.ExpEquipmentId4).HasColumnName("ExpEquipment_id4");
            entity.Property(e => e.Quantity1).HasColumnName("quantity1");
            entity.Property(e => e.Quantity2).HasColumnName("quantity2");
            entity.Property(e => e.Quantity3).HasColumnName("quantity3");
            entity.Property(e => e.Quantity4).HasColumnName("quantity4");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ExpShoppingCart_UserUsers");
        });

        modelBuilder.Entity<ExpSpeciality>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SportsName)
                .HasMaxLength(50)
                .HasColumnName("sports_name");
        });

        modelBuilder.Entity<ExpTransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpTrans__3213E83F43C30E73");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Flow).HasColumnName("flow");
            entity.Property(e => e.ReceiveUserId).HasColumnName("receive_user_id");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.ReceiveUser).WithMany(p => p.ExpTransactionReceiveUsers)
                .HasForeignKey(d => d.ReceiveUserId)
                .HasConstraintName("FK_expTransactions_UserUsers1");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.ExpTransactionSenderUsers)
                .HasForeignKey(d => d.SenderUserId)
                .HasConstraintName("FK_expTransactions_UserUsers");

            entity.HasOne(d => d.Type).WithMany(p => p.ExpTransactions)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK__ExpTransa__type___336AA144");
        });

        modelBuilder.Entity<ExpTransactionsType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpTrans__3213E83FCD7F43F4");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.TransType)
                .HasMaxLength(50)
                .HasColumnName("trans_type");
        });

        modelBuilder.Entity<ExpTthirdPartyPayment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ExpTthird_party_payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemName).HasMaxLength(400);
            entity.Property(e => e.MerchantTradeNo).HasMaxLength(50);
            entity.Property(e => e.RtnMsg).HasMaxLength(200);
            entity.Property(e => e.TradeDesc).HasMaxLength(200);
            entity.Property(e => e.TradeNo).HasMaxLength(50);
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

            entity.HasOne(d => d.Transaction).WithMany()
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_expTthird_party_payments_expTransactions");
        });

        modelBuilder.Entity<ForumAd>(entity =>
        {
            entity.HasKey(e => e.AdId).HasName("PK__forumAds__CAA4A627A6B69F60");

            entity.Property(e => e.AdId).HasColumnName("ad_id");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(500)
                .HasColumnName("img_url");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(500)
                .HasColumnName("link_url");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("REQUEST")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Transaction).WithMany(p => p.ForumAds)
                .HasForeignKey(d => d.TransactionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumAds_expTransactions");

            entity.HasOne(d => d.User).WithMany(p => p.ForumAds)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumAds_UserUsers");
        });

        modelBuilder.Entity<ForumBoardCategory>(entity =>
        {
            entity.HasKey(e => e.BoardId).HasName("PK__forumBoa__FB1C96E9D12BC215");

            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<ForumBoardInteraction>(entity =>
        {
            entity.HasKey(e => e.InteractionId).HasName("PK__forumBoa__605F8FE64959D49A");

            entity.Property(e => e.InteractionId).HasColumnName("interaction_id");
            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Board).WithMany(p => p.ForumBoardInteractions)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumBoardInteractions_ForumBoardCategories");

            entity.HasOne(d => d.User).WithMany(p => p.ForumBoardInteractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumBoardInteractions_UserUsers");
        });

        modelBuilder.Entity<ForumComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__forumCom__E7957687AD99822E");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentCommentId).HasColumnName("parentComment_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.ForumComments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_forumComments_forumPosts");

            entity.HasOne(d => d.User).WithMany(p => p.ForumComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumComments_UserUsers");
        });

        modelBuilder.Entity<ForumPost>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__forumPos__3ED7876687F37592");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsPinned).HasColumnName("is_pinned");
            entity.Property(e => e.IsPosted).HasColumnName("is_posted");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("NORMAL")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Board).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumPosts_ForumBoardCategories");

            entity.HasOne(d => d.Location).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_ForumPosts_TripLocations");

            entity.HasOne(d => d.User).WithMany(p => p.ForumPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumPosts_UserUsers");
        });

        modelBuilder.Entity<ForumPostInteraction>(entity =>
        {
            entity.HasKey(e => e.InteractionId).HasName("PK__forumPos__605F8FE61A2346F2");

            entity.Property(e => e.InteractionId).HasColumnName("interaction_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ReportReason).HasColumnName("report_reason");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Post).WithMany(p => p.ForumPostInteractions)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_forumPostInteraction_forumPosts");

            entity.HasOne(d => d.User).WithMany(p => p.ForumPostInteractions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumPostInteractions_UserUsers");
        });

        modelBuilder.Entity<ForumPostTagDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(d => d.Post).WithMany(p => p.ForumPostTagDetails)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForumPostTagDetails_ForumPosts");

            entity.HasOne(d => d.Tag).WithMany(p => p.ForumPostTagDetails)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_forumPostTagDetail_forumTags");
        });

        modelBuilder.Entity<ForumPostsImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__forumPos__DC9AC95590756DFB");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .HasColumnName("image_url");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.SortIndex).HasColumnName("sort_index");

            entity.HasOne(d => d.Post).WithMany(p => p.ForumPostsImages)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__forumPost__post___07C12930");
        });

        modelBuilder.Entity<ForumSensitiveWord>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PK__forumSen__7FFA1D40FD7923BA");

            entity.Property(e => e.WordId).HasColumnName("word_id");
            entity.Property(e => e.Level)
                .HasMaxLength(20)
                .HasColumnName("level");
            entity.Property(e => e.Word).HasColumnName("word");
        });

        modelBuilder.Entity<ForumTag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__forumTag__4296A2B61086CE8F");

            entity.HasIndex(e => e.TagName, "UQ__forumTag__E298655C3277541C").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasColumnName("tag_name");
        });

        modelBuilder.Entity<HomAmenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK__HomAmeni__5C8402D615B6F704");

            entity.ToTable("HomAmenity");

            entity.Property(e => e.AmenityId).HasColumnName("amenityID");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.IconCode)
                .HasMaxLength(50)
                .HasColumnName("iconCode");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<HomBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__HomBooki__C6D03BED9AA171A9");

            entity.ToTable("HomBooking");

            entity.Property(e => e.BookingId).HasColumnName("bookingID");
            entity.Property(e => e.CancelReason).HasColumnName("cancelReason");
            entity.Property(e => e.CheckInDate)
                .HasColumnType("datetime")
                .HasColumnName("checkInDate");
            entity.Property(e => e.CheckOutDate)
                .HasColumnType("datetime")
                .HasColumnName("checkOutDate");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("createdTime");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.RoomTypeId).HasColumnName("roomTypeID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("totalPrice");
            entity.Property(e => e.TransactionsId).HasColumnName("TransactionsID");
            entity.Property(e => e.UpdateTime)
                .HasColumnType("datetime")
                .HasColumnName("updateTime");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.RoomType).WithMany(p => p.HomBookings)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomBookin__roomT__19FFD4FC");

            entity.HasOne(d => d.Transactions).WithMany(p => p.HomBookings)
                .HasForeignKey(d => d.TransactionsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomBooking_expTransactions");

            entity.HasOne(d => d.User).WithMany(p => p.HomBookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomBooking_UserUsers");
        });

        modelBuilder.Entity<HomCitieMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HomCitieMapping");

            entity.Property(e => e.CityId).HasColumnName("cityID");
            entity.Property(e => e.HouseId).HasColumnName("houseID");

            entity.HasOne(d => d.City).WithMany()
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomCitieMapping_TripCities");

            entity.HasOne(d => d.House).WithMany()
                .HasForeignKey(d => d.HouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomCitieMapping_HomHouse");
        });

        modelBuilder.Entity<HomDistrictMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HomDistrictMapping");

            entity.Property(e => e.DistrictId).HasColumnName("districtID");
            entity.Property(e => e.HouseId).HasColumnName("houseID");

            entity.HasOne(d => d.District).WithMany()
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomDistrictMapping_TripDistricts");

            entity.HasOne(d => d.House).WithMany()
                .HasForeignKey(d => d.HouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomDistrictMapping_HomHouse");
        });

        modelBuilder.Entity<HomHouse>(entity =>
        {
            entity.HasKey(e => e.HouseId).HasName("PK__HomHouse__EC48E02823C62491");

            entity.ToTable("HomHouse");

            entity.Property(e => e.HouseId).HasColumnName("houseID");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UserId).HasColumnName("userID");
        });

        modelBuilder.Entity<HomLocationMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HomLocationMapping");

            entity.Property(e => e.HouseId).HasColumnName("houseID");
            entity.Property(e => e.LocationId).HasColumnName("locationID");

            entity.HasOne(d => d.House).WithMany()
                .HasForeignKey(d => d.HouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomLocationMapping_HomHouse");

            entity.HasOne(d => d.Location).WithMany()
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HomLocationMapping_TripLocations");
        });

        modelBuilder.Entity<HomMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__HomMessa__4808B873E35E5B68");

            entity.ToTable("HomMessage");

            entity.Property(e => e.MessageId).HasColumnName("messageID");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.IsRead).HasColumnName("isRead");
            entity.Property(e => e.ReceiverId).HasColumnName("receiverID");
            entity.Property(e => e.SendTime)
                .HasColumnType("datetime")
                .HasColumnName("sendTime");
            entity.Property(e => e.SenderId).HasColumnName("senderID");
        });

        modelBuilder.Entity<HomReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__HomRevie__2ECD6E241C0DE5B0");

            entity.ToTable("HomReview");

            entity.Property(e => e.ReviewId).HasColumnName("reviewID");
            entity.Property(e => e.BookingId).HasColumnName("bookingID");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("createdTime");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.RoomTypeId).HasColumnName("roomTypeID");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Booking).WithMany(p => p.HomReviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomReview__booki__1AF3F935");

            entity.HasOne(d => d.RoomType).WithMany(p => p.HomReviews)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomReview__roomT__1BE81D6E");
        });

        modelBuilder.Entity<HomRoomCalendar>(entity =>
        {
            entity.HasKey(e => e.CalendarId).HasName("PK__HomRoomC__EE5496D6B538693F");

            entity.ToTable("HomRoomCalendar");

            entity.Property(e => e.CalendarId).HasColumnName("calendarID");
            entity.Property(e => e.IsAvailable).HasColumnName("isAvailable");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("price");
            entity.Property(e => e.RoomTypeId).HasColumnName("roomTypeID");
            entity.Property(e => e.TargetDate).HasColumnName("targetDate");

            entity.HasOne(d => d.RoomType).WithMany(p => p.HomRoomCalendars)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomRoomCa__roomT__17236851");
        });

        modelBuilder.Entity<HomRoomImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__HomRoomI__336E9B756F4560A7");

            entity.ToTable("HomRoomImage");

            entity.Property(e => e.ImageId).HasColumnName("imageID");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("createdTime");
            entity.Property(e => e.ImagePath).HasColumnName("imagePath");
            entity.Property(e => e.RoomTypeId).HasColumnName("roomTypeID");

            entity.HasOne(d => d.RoomType).WithMany(p => p.HomRoomImages)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomRoomIm__roomT__162F4418");
        });

        modelBuilder.Entity<HomRoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__HomRoomT__5E5E0CD3F55F8224");

            entity.ToTable("HomRoomType");

            entity.Property(e => e.RoomTypeId).HasColumnName("roomTypeID");
            entity.Property(e => e.ApplyTime)
                .HasColumnType("datetime")
                .HasColumnName("applyTime");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("createdTime");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HouseId).HasColumnName("houseID");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PricePerNight)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("pricePerNight");
            entity.Property(e => e.RejectReason).HasColumnName("rejectReason");
            entity.Property(e => e.ReviewStatus).HasColumnName("reviewStatus");
            entity.Property(e => e.UpdateTime)
                .HasColumnType("datetime")
                .HasColumnName("updateTime");
            entity.Property(e => e.ViewCount).HasColumnName("viewCount");

            entity.HasOne(d => d.House).WithMany(p => p.HomRoomTypes)
                .HasForeignKey(d => d.HouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomRoomTy__house__153B1FDF");
        });

        modelBuilder.Entity<HomRoomTypeAmenity>(entity =>
        {
            entity.HasKey(e => e.RoomTypeAmenityId).HasName("PK__HomRoomT__F99CF3ADD5FBC46B");

            entity.ToTable("HomRoomTypeAmenity");

            entity.Property(e => e.RoomTypeAmenityId).HasColumnName("roomTypeAmenityID");
            entity.Property(e => e.AmenityId).HasColumnName("amenityID");
            entity.Property(e => e.RoomTypeId).HasColumnName("roomTypeID");

            entity.HasOne(d => d.Amenity).WithMany(p => p.HomRoomTypeAmenities)
                .HasForeignKey(d => d.AmenityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomRoomTy__ameni__190BB0C3");

            entity.HasOne(d => d.RoomType).WithMany(p => p.HomRoomTypeAmenities)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HomRoomTy__roomT__18178C8A");
        });

        modelBuilder.Entity<TripAnnouncement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripAnno__3213E83F2D629271");

            entity.HasIndex(e => e.IsPinned, "IX_TripAnnouncements_Pinned");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.IsPinned).HasColumnName("is_pinned");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.TripAnnouncements)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripAnnouncements_CreatedByUser");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripAnnouncements)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripAnnouncements_Trips");
        });

        modelBuilder.Entity<TripCity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripCiti__3213E83F436EEC54");

            entity.HasIndex(e => e.Name, "TripCities_index_0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<TripDistrict>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripDist__3213E83F566B3740");

            entity.HasIndex(e => e.CityId, "TripDistricts_index_1");

            entity.HasIndex(e => new { e.CityId, e.Name }, "TripDistricts_index_2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.City).WithMany(p => p.TripDistricts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TripDistr__city___7814D14C");
        });

        modelBuilder.Entity<TripFavorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripFavo__3213E83F4766FD43");

            entity.HasIndex(e => new { e.TripId, e.UserId }, "UX_TripFavorites").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripFavorites)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripFavorites_Trips");

            entity.HasOne(d => d.User).WithMany(p => p.TripFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripFavorites_User");
        });

        modelBuilder.Entity<TripGearCheck>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripGear__3213E83F2F7429CF");

            entity.HasIndex(e => new { e.TripGearItemId, e.UserId }, "UX_TripGearChecks").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CheckedAt)
                .HasColumnType("datetime")
                .HasColumnName("checked_at");
            entity.Property(e => e.IsChecked).HasColumnName("is_checked");
            entity.Property(e => e.TripGearItemId).HasColumnName("trip_gear_item_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.TripGearItem).WithMany(p => p.TripGearChecks)
                .HasForeignKey(d => d.TripGearItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripGearChecks_Items");

            entity.HasOne(d => d.User).WithMany(p => p.TripGearChecks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripGearChecks_User");
        });

        modelBuilder.Entity<TripGearItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripGear__3213E83FF3DCD635");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedByUserId).HasColumnName("created_by_user_id");
            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.IsRequired).HasColumnName("is_required");
            entity.Property(e => e.ItemName)
                .HasMaxLength(200)
                .HasColumnName("item_name");
            entity.Property(e => e.TripId).HasColumnName("trip_id");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.TripGearItems)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripGearItems_CreatedByUser");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripGearItems)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripGearItems_Trips");
        });

        modelBuilder.Entity<TripLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripLoca__3213E83FAE9536F4");

            entity.HasIndex(e => e.DistrictId, "TripLocations_index_3");

            entity.HasIndex(e => new { e.Lat, e.Lng }, "TripLocations_index_4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressText)
                .HasMaxLength(500)
                .HasColumnName("address_text");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.GooglePlaceId)
                .HasMaxLength(300)
                .HasColumnName("google_place_id");
            entity.Property(e => e.Lat)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("lat");
            entity.Property(e => e.Lng)
                .HasColumnType("decimal(9, 6)")
                .HasColumnName("lng");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.City).WithMany(p => p.TripLocations)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripLocations_TripCities");

            entity.HasOne(d => d.District).WithMany(p => p.TripLocations)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripLocations_TripDistricts");
        });

        modelBuilder.Entity<TripMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripMemb__3213E83F5E29E8D2");

            entity.HasIndex(e => new { e.TripId, e.UserId }, "UX_TripMembers_Trip_User").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripMembers)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripMembers_Trips");

            entity.HasOne(d => d.User).WithMany(p => p.TripMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripMembers_User");
        });

        modelBuilder.Entity<TripNote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripNote__3213E83FF7197CE4");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.TimelineId).HasColumnName("timeline_id");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(e => e.Version)
                .HasDefaultValue(1)
                .HasColumnName("version");

            entity.HasOne(d => d.Timeline).WithMany(p => p.TripNotes)
                .HasForeignKey(d => d.TimelineId)
                .HasConstraintName("FK_TripNotes_Timeline");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripNotes)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripNotes_Trips");

            entity.HasOne(d => d.UpdatedByUser).WithMany(p => p.TripNotes)
                .HasForeignKey(d => d.UpdatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripNotes_User");
        });

        modelBuilder.Entity<TripReminder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripRemi__3213E83F79CAB5BE");

            entity.HasIndex(e => new { e.TripId, e.UserId, e.RemindOffsetMinutes }, "UX_TripReminders").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(true)
                .HasColumnName("is_enabled");
            entity.Property(e => e.LastSentAt)
                .HasColumnType("datetime")
                .HasColumnName("last_sent_at");
            entity.Property(e => e.RemindOffsetMinutes).HasColumnName("remind_offset_minutes");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripReminders)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripReminders_Trips");
        });

        modelBuilder.Entity<TripTimeline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripTime__3213E83FF5CB1CD2");

            entity.ToTable("TripTimeline");

            entity.HasIndex(e => new { e.TripId, e.SortOrder }, "UX_TripTimeline_Sort").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.TripLocationId).HasColumnName("trip_location_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripTimelines)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripTimeline_Trips");

            entity.HasOne(d => d.TripLocation).WithMany(p => p.TripTimelines)
                .HasForeignKey(d => d.TripLocationId)
                .HasConstraintName("FK_TripTimeline_TripLocation");
        });

        modelBuilder.Entity<TripTrip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripTrip__3213E83F94C72B9D");

            entity.HasIndex(e => e.OrganizerUserId, "IX_TripTrips_Organizer");

            entity.HasIndex(e => e.StartAt, "IX_TripTrips_StartAt");

            entity.HasIndex(e => e.Status, "IX_TripTrips_Status");

            entity.HasIndex(e => e.TripType, "IX_TripTrips_Type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CoverImagePublicId)
                .HasMaxLength(200)
                .HasColumnName("cover_image_public_id");
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(500)
                .HasColumnName("cover_image_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndAt)
                .HasColumnType("datetime")
                .HasColumnName("end_at");
            entity.Property(e => e.LockedAt).HasColumnName("locked_at");
            entity.Property(e => e.OrganizerUserId).HasColumnName("organizer_user_id");
            entity.Property(e => e.StartAt)
                .HasColumnType("datetime")
                .HasColumnName("start_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.TripType)
                .HasMaxLength(50)
                .HasColumnName("trip_type");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.OrganizerUser).WithMany(p => p.TripTrips)
                .HasForeignKey(d => d.OrganizerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripTrips_UserUsers");
        });

        modelBuilder.Entity<TripTripLocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripTrip__3213E83FDFFF3B7D");

            entity.HasIndex(e => new { e.TripId, e.SortOrder }, "UX_TripTripLocations_Sort").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.LocationRole)
                .HasMaxLength(50)
                .HasColumnName("location_role");
            entity.Property(e => e.Note)
                .HasMaxLength(500)
                .HasColumnName("note");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.TripId).HasColumnName("trip_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Location).WithMany(p => p.TripTripLocations)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripTripLocations_Location");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripTripLocations)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripTripLocations_Trips");
        });

        modelBuilder.Entity<UserAccountDeletion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAcco__3213E83FC905C943");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.RequestedByAdminId).HasColumnName("requested_by_admin_id");
            entity.Property(e => e.RequestedByUserId).HasColumnName("requested_by_user_id");

            entity.HasOne(d => d.Account).WithMany(p => p.UserAccountDeletionAccounts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAccountDeletions_UserUsers1");

            entity.HasOne(d => d.RequestedByAdmin).WithMany(p => p.UserAccountDeletions)
                .HasForeignKey(d => d.RequestedByAdminId)
                .HasConstraintName("FK__UserAccou__reque__39237A9A");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.UserAccountDeletionRequestedByUsers)
                .HasForeignKey(d => d.RequestedByUserId)
                .HasConstraintName("FK_UserAccountDeletions_UserUsers2");
        });

        modelBuilder.Entity<UserAdmin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAdmi__3213E83F9CB291F9");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminName)
                .HasMaxLength(255)
                .HasColumnName("admin_name");
            entity.Property(e => e.AdminRoleId).HasColumnName("admin_role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PictureId).HasColumnName("picture_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.AdminRole).WithMany(p => p.UserAdmins)
                .HasForeignKey(d => d.AdminRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAdmins_UserAdminRoles");

            entity.HasOne(d => d.Picture).WithMany(p => p.UserAdmins)
                .HasForeignKey(d => d.PictureId)
                .HasConstraintName("FK_UserAdmins_UserPictures");

            entity.HasOne(d => d.Status).WithMany(p => p.UserAdmins)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserAdmins_UserStatuses");
        });

        modelBuilder.Entity<UserAdminRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAdmi__3213E83FE31D13E6");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserFaqAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserFaqA__3213E83FEA6384AF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Answer).HasColumnName("answer");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Question)
                .HasMaxLength(255)
                .HasColumnName("question");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.UserFaqAnswers)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__UserFaqAn__categ__3FD07829");
        });

        modelBuilder.Entity<UserPicture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPict__3213E83FAFB95D81");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Picture).HasColumnName("picture");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserSocialAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSoci__3213E83F4EF880CE");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("access_token");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expires_at");
            entity.Property(e => e.Provider)
                .HasMaxLength(50)
                .HasColumnName("provider");
            entity.Property(e => e.ProviderUserId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("provider_user_id");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("refresh_token");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserSocialAccounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSocia__user___308E3499");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserStat__3213E83F6FC46315");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserSupportMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSupp__3213E83F70C5DF0A");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgentId).HasColumnName("agent_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MatchedAnswerId).HasColumnName("matched_answer_id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.SenderType)
                .HasMaxLength(50)
                .HasColumnName("sender_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Agent).WithMany(p => p.UserSupportMessages)
                .HasForeignKey(d => d.AgentId)
                .HasConstraintName("FK__UserSuppo__agent__3DE82FB7");

            entity.HasOne(d => d.MatchedAnswer).WithMany(p => p.UserSupportMessages)
                .HasForeignKey(d => d.MatchedAnswerId)
                .HasConstraintName("FK_UserSupportMessages_UserFaqAnswers");

            entity.HasOne(d => d.User).WithMany(p => p.UserSupportMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSupportMessages_UserUsers");
        });

        modelBuilder.Entity<UserSupportTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSupp__3213E83F06A33D33");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAgentId).HasColumnName("assigned_agent_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedAgent).WithMany(p => p.UserSupportTickets)
                .HasForeignKey(d => d.AssignedAgentId)
                .HasConstraintName("FK_UserSupportTickets_UserAdmins");

            entity.HasOne(d => d.Category).WithMany(p => p.UserSupportTickets)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSuppo__categ__3B0BC30C");

            entity.HasOne(d => d.Status).WithMany(p => p.UserSupportTickets)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSuppo__statu__3BFFE745");

            entity.HasOne(d => d.User).WithMany(p => p.UserSupportTickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserSuppo__user___3A179ED3");
        });

        modelBuilder.Entity<UserSupportTicketCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserSupp__3213E83F16C7A6C6");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserUser__3213E83F4AFD6C0F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailVerificationExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("email_verification_expires_at");
            entity.Property(e => e.EmailVerificationOtp)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("email_verification_otp");
            entity.Property(e => e.EmailVerifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("email_verified_at");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.PasswordChangedAt)
                .HasColumnType("datetime")
                .HasColumnName("password_changed_at");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordResetExpiresAt)
                .HasColumnType("datetime")
                .HasColumnName("password_reset_expires_at");
            entity.Property(e => e.PasswordResetOtp)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("password_reset_otp");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("user_name");
            entity.Property(e => e.UserRoleId).HasColumnName("user_role_id");

            entity.HasOne(d => d.Status).WithMany(p => p.UserUsers)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserUsers__statu__318258D2");

            entity.HasOne(d => d.UserRole).WithMany(p => p.UserUsers)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserUsers__user___336AA144");
        });

        modelBuilder.Entity<UserUserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserUser__3213E83F4ED27973");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
