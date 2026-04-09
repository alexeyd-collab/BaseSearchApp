using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SearchApp.Models;

namespace SearchApp.Services.Storage
{
    public class DistributedCacheSearchResultStorage : ISearchResultStorage
    {
        private readonly IDistributedCache _cache;

        public DistributedCacheSearchResultStorage(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<List<RepoSearchItem>?> GetAsync(string keyword)
        {
            var json = await _cache.GetStringAsync(keyword);
            if (string.IsNullOrEmpty(json))
                return null;

            return JsonSerializer.Deserialize<List<RepoSearchItem>>(json);
        }

        public async Task SetAsync(string keyword, List<RepoSearchItem> results)
        {
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            
            var json = JsonSerializer.Serialize(results);
            await _cache.SetStringAsync(keyword, json, options);
        }
    }
}
