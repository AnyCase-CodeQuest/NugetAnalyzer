using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Common.Configurations;
namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PackageVersionConfigurations packageVersionConfigurations;

        public HomeController(IOptions<PackageVersionConfigurations> packageVersionConfigurations)
        {
            this.packageVersionConfigurations = (packageVersionConfigurations ?? throw new ArgumentNullException(nameof(packageVersionConfigurations))).Value;
        }
        public IActionResult Index()
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

        public IActionResult Rules()
        {
            return View(packageVersionConfigurations);
        }
    }
}