namespace HackerNewsBestStoriesApi.Models
{
    public class Story
    {
        public required int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? By { get; set; }
        public long? Time { get; set; }
        public int? Score { get; set; }
        public int? Descendants { get; set; }
    }
}