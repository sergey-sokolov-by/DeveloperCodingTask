using HackerNewsBestStoriesApi.Models;

namespace HackerNewsBestStoriesApi.Services
{
    public interface IHackerNewsService
    {
        Task<List<StoryDto>> GetBestStoriesAsync(int limit);
    }
}