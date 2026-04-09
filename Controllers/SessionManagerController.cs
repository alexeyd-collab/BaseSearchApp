using SearchApp.Models;
using SearchApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SearchApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionManagerController : ControllerBase
    {
        private readonly ISessionManagerService _sessionManagerService;

        public SessionManagerController(ISessionManagerService sessionManagerService)
        {
            _sessionManagerService = sessionManagerService;
        }

        [HttpPost]
        public async Task SaveToSession(string name)
        {
            await _sessionManagerService.Save(name);
        }
    }
}
