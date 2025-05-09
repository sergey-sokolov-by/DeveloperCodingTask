namespace HackerNewsBestStoriesApi.Infrastructure
{
    public static class CacheKeys
    {
        public const string BestStories = "beststories";
        public static string Story(int id) => $"story:{id}";
    }
}
