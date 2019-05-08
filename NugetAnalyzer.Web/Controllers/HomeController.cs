using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private IRepository Repository { get; }
        public HomeController(IRepository repository)
        {
            Repository = repository;
        }
        public IActionResult Index()
        {
            Repository.Save(null, 1);
            return View();
        }
    }
}