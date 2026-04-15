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
        public async Task<IActionResult> RepoSearch([FromBody] string keyword)
        {
            try
            {
                var results = await _repoSearcService.Search(keyword);
                return Ok(results);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(502, new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An unexpected error occurred while processing your search. Please try again." });
            }
        }
    }
}
