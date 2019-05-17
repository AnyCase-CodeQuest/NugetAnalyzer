using Microsoft.AspNetCore.Mvc;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}