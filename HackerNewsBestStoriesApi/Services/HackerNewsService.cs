using HackerNewsBestStoriesApi.Infrastructure;
using HackerNewsBestStoriesApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNewsBestStoriesApi.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IHackerNewsClient _client;
        private readonly IMemoryCache _cache;
        private readonly HackerNewsOptions _options;
        private readonly ILogger<HackerNewsService> _logger;

        public HackerNewsService(
            IHackerNewsClient client,
            IMemoryCache cache,
            IOptions<HackerNewsOptions> options,
            ILogger<HackerNewsService> logger)
        {
            _client = client;
            _cache = cache;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<List<StoryDto>> GetBestStoriesAsync(int limit)
        {
            var storyIds = await GetStoryIdsAsync();
            var selectedIds = storyIds.Take(limit);

            var semaphore = new SemaphoreSlim(_options.MaxConcurrentRequests);
            var tasks = selectedIds.Select(id => LoadStoryWithSemaphoreAsync(id, semaphore));
            var results = await Task.WhenAll(tasks);

            return results
                .Where(s => s != null)
                .OrderByDescending(s => s!.Score)
                .ToList()!;
        }

        private async Task<int[]> GetStoryIdsAsync()
        {
            if (_cache.TryGetValue(CacheKeys.BestStories, out int[]? cachedStoryIds))
            {
                return cachedStoryIds!;
            }

            _logger.LogInformation("Fetching best stories IDs from Hacker News API");
            var storyIds = await _client.GetBestStoryIdsAsync();
            if (storyIds == null || storyIds.Length == 0)
            {
                _logger.LogWarning("Failed to fetch best stories list");
                return Array.Empty<int>();
            }

            _cache.Set(CacheKeys.BestStories, storyIds, TimeSpan.FromSeconds(_options.BestStoriesCacheTimeout));

            return storyIds;
        }

        private async Task<StoryDto?> LoadStoryWithSemaphoreAsync(int id, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                return await LoadStoryAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching story with ID {Id}", id);
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<StoryDto?> LoadStoryAsync(int id)
        {
            if (_cache.TryGetValue(CacheKeys.Story(id), out StoryDto? story))
            {
                return story;
            }

            var url = string.Format(_options.ItemUrlTemplate, id);
            var storyData = await _client.GetStoryByIdAsync(id);

            if (storyData?.Score == null)
            {
                _logger.LogWarning("Story {Id} is null or has no score and is excluded", id);
                return null;
            }

            story = new StoryDto
            {
                Title = storyData.Title,
                Uri = storyData.Url,
                PostedBy = storyData.By,
                Time = storyData.Time.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(storyData.Time.Value).ToString("yyyy-MM-dd'T'HH:mm:sszzz")
                    : null,
                Score = storyData.Score.Value,
                CommentCount = storyData.Descendants
            };

            _cache.Set(CacheKeys.Story(id), story, TimeSpan.FromMinutes(_options.StoryCacheTimeout));
            _logger.LogDebug("Cached story {StoryId}", id);

            return story;
        }
    }
}