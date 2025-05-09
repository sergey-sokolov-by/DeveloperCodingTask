using System;
using System.Threading.Tasks;
using HackerNewsBestStoriesApi.Models;
using HackerNewsBestStoriesApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HackerNewsBestStoriesApi.Tests.Services;

public class HackerNewsServiceTests
{
    private readonly Mock<IHackerNewsClient> _clientMock = new();
    private readonly Mock<ILogger<HackerNewsService>> _loggerMock = new();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly IOptions<HackerNewsOptions> _options =
        Options.Create(new HackerNewsOptions
        {
            BestStoriesUrl = "https://hacker-news.firebaseio.com/v0/beststories.json",
            ItemUrlTemplate = "https://hacker-news.firebaseio.com/v0/item/{0}.json",
            BestStoriesCacheTimeout = 10,
            StoryCacheTimeout = 60,
            MaxConcurrentRequests = 10
        });

    [Fact]
    public async Task GetBestStoriesAsync_ReturnsExpectedStories()
    {
        _clientMock.Setup(c => c.GetBestStoryIdsAsync()).ReturnsAsync(new[] { 1, 2 });
        _clientMock.Setup(c => c.GetStoryByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new Story
            {
                Id = id,
                Title = $"Story {id}",
                By = "author",
                Url = "http://example.com",
                Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Score = 100 + id,
                Descendants = 42
            });

        var service = new HackerNewsService(_clientMock.Object, _cache, _options, _loggerMock.Object);
        var result = await service.GetBestStoriesAsync(2);

        Assert.Equal(2, result.Count);
        Assert.All(result, story => Assert.True(story.Score > 100));
    }

    [Fact]
    public async Task GetBestStoriesAsync_HandlesNullStory()
    {
        _clientMock.Setup(c => c.GetBestStoryIdsAsync()).ReturnsAsync(new[] { 1 });
        _clientMock.Setup(c => c.GetStoryByIdAsync(1)).ReturnsAsync((Story?)null);

        var service = new HackerNewsService(_clientMock.Object, _cache, _options, _loggerMock.Object);
        var result = await service.GetBestStoriesAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBestStoriesAsync_HandlesMissingScore()
    {
        _clientMock.Setup(c => c.GetBestStoryIdsAsync()).ReturnsAsync(new[] { 1 });
        _clientMock.Setup(c => c.GetStoryByIdAsync(1)).ReturnsAsync(new Story
        {
            Id = 1,
            Title = "No Score",
            By = "someone",
            Url = "http://example.com",
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Score = null,
            Descendants = 0
        });

        var service = new HackerNewsService(_clientMock.Object, _cache, _options, _loggerMock.Object);
        var result = await service.GetBestStoriesAsync(1);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBestStoriesAsync_ZeroLimit_ReturnsEmpty()
    {
        var service = new HackerNewsService(_clientMock.Object, _cache, _options, _loggerMock.Object);
        var result = await service.GetBestStoriesAsync(0);
        Assert.Empty(result);
    }
}
