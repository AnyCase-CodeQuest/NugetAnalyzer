using Microsoft.AspNetCore.Mvc;

namespace NugetAnalyzer.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult NotFoundError()
        {
            return View();
        }
    }
}