using Microsoft.Extensions.Options;
using SearchApp.Models;

namespace SearchApp.Services.Storage
{
    public interface ISearchResultStorageFactory
    {
        ISearchResultStorage GetStorage();
    }

    public class SearchResultStorageFactory : ISearchResultStorageFactory
    {
        private readonly StorageSettings _settings;
        private readonly IServiceProvider _serviceProvider;

        public SearchResultStorageFactory(IOptions<StorageSettings> settings, IServiceProvider serviceProvider)
        {
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
        }

        public ISearchResultStorage GetStorage()
        {
            if (_settings.Provider == StorageType.DistributedCache)
            {
                return _serviceProvider.GetRequiredService<DistributedCacheSearchResultStorage>();
            }

            return _serviceProvider.GetRequiredService<MemorySearchResultStorage>();
        }
    }
}
