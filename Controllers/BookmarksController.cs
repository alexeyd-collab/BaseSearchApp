using SearchApp.Models;
using SearchApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace SearchApp.Controllers
{
    public class BookmarksController : Controller
    {
        public BookmarksController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
