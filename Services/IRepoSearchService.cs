using SearchApp.Models;

namespace SearchApp.Services
{
    public interface IRepoSearchService
    {
        Task<List<RepoSearchItem>> Search(string keyword);
    }
}
