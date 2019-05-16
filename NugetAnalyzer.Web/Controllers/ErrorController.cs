using Microsoft.AspNetCore.Mvc;

namespace NugetAnalyzer.Web.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("404")]
        public ActionResult NotFoundError()
        {
            return View();
        }
    }
}