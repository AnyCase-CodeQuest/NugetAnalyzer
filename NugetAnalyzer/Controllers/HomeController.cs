using Microsoft.AspNetCore.Mvc;

namespace NugetAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}