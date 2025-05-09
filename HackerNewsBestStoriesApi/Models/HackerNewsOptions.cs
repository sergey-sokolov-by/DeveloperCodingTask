namespace HackerNewsBestStoriesApi.Models
{
    public class HackerNewsOptions
    {
        public int BestStoriesCacheTimeout { get; set; }
        public string BestStoriesUrl { get; set; } = string.Empty;
        public string ItemUrlTemplate { get; set; } = string.Empty;
        public int StoryCacheTimeout { get; set; }
        public int MaxConcurrentRequests { get; set; }
    }
}
