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
                _logger.LogInformation($"Cache hit for keyword: {keyword}");
                return cachedResults;
            }

            _logger.LogInformation($"Cache miss for keyword: {keyword}. Fetching from GitHub API.");
            
            try
            {
                var searchResults = await _httpClient.GetAsync(_settings.BaseUrl + keyword);
                
                if (searchResults.IsSuccessStatusCode)
                {
                    var response = await searchResults.Content.ReadFromJsonAsync<GitHubSearchResponse>();
                    var items = response?.Items ?? new List<RepoSearchItem>();
                    
                    await storage.SetAsync(keyword, items);
                    return items;
                }
                
                _logger.LogWarning($"GitHub API request failed with status code: {searchResults.StatusCode}");
                throw new ApplicationException($"We couldn't retrieve results from GitHub at this time (Status: {searchResults.StatusCode}). Please try again later.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error contacting GitHub API.");
                throw new ApplicationException("We had a problem contacting GitHub. Please check your connection and try again.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout contacting GitHub API.");
                throw new ApplicationException("The search request to GitHub timed out. Please try again later.", ex);
            }
        }
    }
}
