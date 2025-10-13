namespace BookTrackerAPI.Models.DTOs {
    public class HardcoverBookPayload {
    public string EventType { get; set; } = string.Empty;
        public VolumeInfo volumeInfo {get; set;} = null!;
    }

    public class VolumeInfo {
        public string Title {get; set;} = string.Empty;
        public List<string> Authors {get; set;} = new List<string>();
        public string Publisher {get; set;} = string.Empty;
        public string PublishedDate {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public string IndustryIdentifier {get; set;} = string.Empty; // For ISBN
    }
}