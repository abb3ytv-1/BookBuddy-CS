namespace BookTrackerAPI.Models.DTOs
{
    public class AchievementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int Goal { get; set; }
        public string Metric { get; set; } = "";
        public int PointsReward { get; set; }
        public string? IconUrl { get; set; }
        public bool Unlocked { get; set; }
        public int ProgressValue { get; set; }

        public AchievementDto() { }

        public AchievementDto(Achievement achievement)
        {
            Id = achievement.Id;
            Title = achievement.Title;
            Description = achievement.Description;
            Metric = achievement.Metric;
            Goal = achievement.Goal;
            PointsReward = achievement.PointsReward;
            IconUrl = achievement.IconUrl;
        }
    }
}