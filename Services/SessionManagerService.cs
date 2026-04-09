namespace SearchApp.Services
{
    public class SessionManagerService : ISessionManagerService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessionManagerService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Save(string name)
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            if (session != null)
                session.SetString(name, name);
        }
    }
}
