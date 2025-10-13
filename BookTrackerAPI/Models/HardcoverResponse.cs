namespace BookTrackerAPI.Models {
    public class HardcoverResponse {
        public List<HardcoverBook> Books { get; set; } = new();
    }

    public class HardcoverBook {
        public string Title { get; set;} = string.Empty;
        public List<string> Authors { get; set;} = new List<string>();
        public string Description { get; set;} = string.Empty;
        public string Isbn { get; set;} = string.Empty;
        public DateTime? PublishedDate { get; set;}
        public int? pageCount { get; set;}
        public string CoverImageUrl { get; set;} = string.Empty;
    }

    public class Author {
        public string Name { get; set;} = string.Empty;
    }
}