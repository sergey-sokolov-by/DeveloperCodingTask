
using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsBestStoriesApi.Controllers;
using HackerNewsBestStoriesApi.Models;
using HackerNewsBestStoriesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HackerNewsBestStoriesApi.Tests.Controllers;

public class BestStoriesControllerTests
{
    [Fact]
    public async Task GetTopStories_ReturnsOkResult_WithStories()
    {
        var mockService = new Mock<IHackerNewsService>();
        mockService.Setup(s => s.GetBestStoriesAsync(3)).ReturnsAsync(new List<StoryDto>
        {
            new StoryDto { Title = "Test", Score = 100, PostedBy = "me", CommentCount = 10, Time = "2023-01-01T00:00:00+00:00", Uri = "http://x" }
        });

        var controller = new BestStoriesController(mockService.Object);
        var result = await controller.GetTopStories(3);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var stories = Assert.IsAssignableFrom<List<StoryDto>>(okResult.Value);
        Assert.Single(stories);
    }
}
