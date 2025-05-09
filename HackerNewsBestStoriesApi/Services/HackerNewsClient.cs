using HackerNewsBestStoriesApi.Models;
using Microsoft.Extensions.Options;

namespace HackerNewsBestStoriesApi.Services
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly HackerNewsOptions _options;

        public HackerNewsClient(HttpClient httpClient, IOptions<HackerNewsOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<int[]> GetBestStoryIdsAsync()
        {
            return await _httpClient.GetFromJsonAsync<int[]>(_options.BestStoriesUrl) ?? Array.Empty<int>();
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            var url = string.Format(_options.ItemUrlTemplate, id);
            return await _httpClient.GetFromJsonAsync<Story>(url);
        }
    }
}
