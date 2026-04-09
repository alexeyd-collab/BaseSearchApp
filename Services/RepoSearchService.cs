using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SearchApp.Models;
using SearchApp.Services.Storage;

namespace SearchApp.Services
{
    public class GitHubSearchResponse
    {
        [JsonPropertyName("items")]
        public List<RepoSearchItem> Items { get; set; } = new();
    }

    public class RepoSearchService : IRepoSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly GitHubApiSettings _settings;
        private readonly ILogger<RepoSearchService> _logger;
        private readonly ISearchResultStorageFactory _storageFactory;

        public RepoSearchService(
            HttpClient httpClient,
            IOptions<GitHubApiSettings> options,
            ILogger<RepoSearchService> logger,
            ISearchResultStorageFactory storageFactory)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
            _storageFactory = storageFactory;
        }

        public async Task<List<RepoSearchItem>> Search(string keyword)
        {
            var storage = _storageFactory.GetStorage();
            
            var cachedResults = await storage.GetAsync(keyword);
            if (cachedResults != null)
            {
                _logger.LogInformation(SearchApp.Constants.AppConstants.LogMessages.CacheHit, keyword);
                return cachedResults;
            }

            _logger.LogInformation(SearchApp.Constants.AppConstants.LogMessages.CacheMiss, keyword);
            
            var searchResults = await _httpClient.GetAsync(_settings.BaseUrl + keyword);
            
            if (searchResults.IsSuccessStatusCode)
            {
                var response = await searchResults.Content.ReadFromJsonAsync<GitHubSearchResponse>();
                var items = response?.Items ?? new List<RepoSearchItem>();
                
                await storage.SetAsync(keyword, items);
                return items;
            }
            
            _logger.LogWarning(SearchApp.Constants.AppConstants.LogMessages.ApiRequestFailed, searchResults.StatusCode);
            return new List<RepoSearchItem>();
        }
    }
}
