using HackerNewsBestStoriesApi.Models;

namespace HackerNewsBestStoriesApi.Services
{
    public interface IHackerNewsClient
    {
        Task<int[]> GetBestStoryIdsAsync();
        Task<Story?> GetStoryByIdAsync(int id);
    }
}
