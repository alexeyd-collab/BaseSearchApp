using Microsoft.Extensions.Caching.Memory;
using SearchApp.Models;

namespace SearchApp.Services.Storage
{
    public class MemorySearchResultStorage : ISearchResultStorage
    {
        private readonly IMemoryCache _cache;

        public MemorySearchResultStorage(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<List<RepoSearchItem>?> GetAsync(string keyword)
        {
            _cache.TryGetValue(keyword, out List<RepoSearchItem>? results);
            return Task.FromResult(results);
        }

        public Task SetAsync(string keyword, List<RepoSearchItem> results)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(keyword, results, cacheEntryOptions);
            return Task.CompletedTask;
        }
    }
}
