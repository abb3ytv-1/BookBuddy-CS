using BookTrackerAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookTrackerAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
        public DbSet<PrivacySettings> PrivacySettings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SocialPost> SocialPosts { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Book Configuration
            builder.Entity<Book>(b =>
            {
                b.Property(x => x.Title).HasMaxLength(200).IsRequired();
                b.Property(x => x.Author).HasMaxLength(100).IsRequired();
                b.Property(x => x.ISBN).HasMaxLength(20);
                b.HasIndex(x => x.Title);
            });

            // AppUser Configuration
            builder.Entity<AppUser>(b =>
            {
                b.Property(x => x.Bio).HasMaxLength(500);
                b.Property(x => x.ProfilePictureUrl).HasMaxLength(512);
                b.HasIndex(x => x.Email); // Already present
            });

            // UserBook Configuration
            builder.Entity<UserBook>(b =>
            {
                b.HasKey(ub => new { ub.UserId, ub.BookId });

                // Add indexes to speed up filtering
                b.HasIndex(ub => ub.UserId);
                b.HasIndex(ub => ub.BookId);
            });

            // Friendship Configuration
            builder.Entity<Friendship>(b =>
            {
                b.HasKey(f => new { f.UserId, f.FriendId });

                b.HasOne(f => f.User)
                    .WithMany(u => u.Friends)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(f => f.Friend)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FriendId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Add indexes
                b.HasIndex(f => f.UserId);
                b.HasIndex(f => f.FriendId);
            });

            builder.Entity<Follower>(b =>
            {
                b.HasOne(f => f.FollowerUser)
                    .WithMany()
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(f => f.FollowingUser)
                    .WithMany()
                    .HasForeignKey(f => f.FollowingId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Add indexes
                b.HasIndex(f => f.FollowerId);
                b.HasIndex(f => f.FollowingId);
            });

            // Achievement Configuration
            builder.Entity<Achievement>(a =>
            {
                a.Property(x => x.Title).IsRequired().HasMaxLength(100);
                a.Property(x => x.Description).HasMaxLength(500);
                a.Property(x => x.IconUrl).HasMaxLength(512);
                a.Property(x => x.Metric).IsRequired().HasMaxLength(50);
            });

            builder.Entity<UserAchievement>(b =>
            {
                b.HasOne(ua => ua.User)
                    .WithMany(u => u.UserAchievements)
                    .HasForeignKey(ua => ua.UserId);

                b.HasOne(ua => ua.Achievement)
                    .WithMany(a => a.UserAchievements)
                    .HasForeignKey(ua => ua.AchievementId);

                // Add indexes
                b.HasIndex(ua => ua.UserId);
                b.HasIndex(ua => ua.AchievementId);
            });
        }
    }
}
