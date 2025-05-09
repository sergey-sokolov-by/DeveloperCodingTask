using HackerNewsBestStoriesApi.Models;
using HackerNewsBestStoriesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace HackerNewsBestStoriesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BestStoriesController : ControllerBase
    {
        private readonly IHackerNewsService _service;

        public BestStoriesController(IHackerNewsService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves the top N Hacker News best stories sorted by score.
        /// </summary>
        /// <param name="count">The number of top best stories to return (e.g. top/10).</param>
        /// <returns>A list of stories.</returns>
        [HttpGet("top/{limit:int}")]
        [SwaggerOperation(Summary = "Get top N of best stories", Description = "Returns the top N best stories from Hacker News based on score.")]
        [ProducesResponseType(typeof(List<StoryDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTopStories([FromRoute, Range(1, 200)] int limit)
        {
            var stories = await _service.GetBestStoriesAsync(limit);
            return Ok(stories);
        }
    }
}