using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitHubApiService gitHubApiService;
        private const string userTestToken = "c515ab303b56798412c4fff4e36f84fdcb3898e2";

        public HomeController(IGitHubApiService gitHubApiService)
        {
            this.gitHubApiService = gitHubApiService;
        }

        public ViewResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> NewRepository()
        {
            var result = await gitHubApiService.GetUserRepositoriesAsync(userTestToken);
            //TODO: exclude already added repos
            return PartialView("RepositoriesPopUp", result);
        }

        [HttpGet]
        public async Task<JsonResult> Branches([FromHeader]long repositoryId)
        {
            var branchesNames = (await gitHubApiService.GetUserRepositoryBranchesAsync(userTestToken, repositoryId))
                .Select(branch => branch.Name);
            return Json(branchesNames);
        }
    }
}