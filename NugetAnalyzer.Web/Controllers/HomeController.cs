using System;
using Microsoft.AspNetCore.Mvc;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}