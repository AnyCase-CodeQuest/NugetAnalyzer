using Microsoft.AspNetCore.Mvc;

namespace NugetAnalyzer.Web.Controllers
{
    public class OAuthSignInController : Controller
    {
        [HttpGet]
        public IActionResult SourceChoice()
        {
            return View();
        }
    }
}