using BookTrackerAPI.Models;

namespace BookTrackerAPI.Data
{
    public static class DataSeeder
    {
        public static void SeedAchievements(AppDbContext context)
        {
            if (context.Achievements.Any()) return; // Skip if achievements already exist!

            var achievements = new List<Achievement>
            {
                new Achievement
                {
                    Title = "First Book Read",
                    Description = "Finish your first book.",
                    Metric = "BooksRead",
                    Goal = 1,
                    PointsReward = 10,
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/1828/1828884.png"
                },
                new Achievement
                {
                    Title = "Reading Streak",
                    Description = "Log in and read for 7 days in a row!",
                    Metric = "LoginStreak",
                    Goal = 7,
                    PointsReward = 50,
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/684/684908.png"
                },
                new Achievement
                {
                    Title = "Book Collector",
                    Description = "Add 20 books to your library.",
                    Metric = "TotalBooks",
                    Goal = 50,
                    PointsReward = 30,
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/3319/3319787.png"
                },
                new Achievement
                {
                    Title = "Points Master",
                    Description = "Accumulate 500 points in total.",
                    Metric = "Points",
                    Goal = 500,
                    PointsReward = 100,
                    IconUrl = "https://cdn-icons-png.flaticon.com/512/753/753318.png"
                }
            };

            context.Achievements.AddRange(achievements);
            context.SaveChanges();
        }

        public static void SeedSocialPosts(AppDbContext context)
        {
            if (context.SocialPosts.Any()) return; // Skip if already seeded

            // Example user ID - update with an actual user ID from your database
            var exampleUserId = "b7b56d39-a074-442d-8e4d-ae8e915278d8";

            var posts = new List<SocialPost>
            {
                new SocialPost
                {
                    Content = "Just finished 'The Hobbit'! 🧙‍♂️",
                    UserId = exampleUserId
                },
                new SocialPost
                {
                    Content = "Started reading 'Dune' today 🌵",
                    UserId = exampleUserId
                },
                new SocialPost
                {
                    Content = "Achieved the 'Book Worm' badge! 🏅",
                    UserId = exampleUserId
                }
            };

            context.SocialPosts.AddRange(posts);
            context.SaveChanges();
        }
    }
}
