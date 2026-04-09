using SearchApp.Models;
using SearchApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace SearchApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepoSearchController : ControllerBase
    {
        private readonly IRepoSearchService _repoSearcService;

        public RepoSearchController(IRepoSearchService repoSearcService)
        {
            _repoSearcService = repoSearcService;
        }

        [HttpPost]
        public async Task<List<RepoSearchItem>> RepoSearch([FromBody] string keyword)
        {
            return await _repoSearcService.Search(keyword);
        }
    }
}
