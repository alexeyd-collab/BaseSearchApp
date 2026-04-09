using SearchApp.Models;

namespace SearchApp.Services.Storage
{
    public interface ISearchResultStorage
    {
        Task<List<RepoSearchItem>?> GetAsync(string keyword);
        Task SetAsync(string keyword, List<RepoSearchItem> results);
    }
}
