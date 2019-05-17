using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitHubApiService gitHubApiService;
        private const string userTestToken = "8cb0a209315ca738d9c5af0eacf392442a7947b8";

        public HomeController(IGitHubApiService gitHubApiService)
        {
            this.gitHubApiService = gitHubApiService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> AddRepositories()
        {
            var result = await gitHubApiService.GetUserRepositoriesAsync(userTestToken);
            //TODO: exclude already added repos
            return PartialView("RepositoriesPopUp", result);
        }

        [HttpPost]
        public async Task<PartialViewResult> AddRepositories(Dictionary<string, string> repositories)
        {

            return PartialView("RepositoriesPopUp", null);
        }

        [HttpGet]
        public async Task<JsonResult> Branches(long repositoryId)
        {
            var branchesNames = (await gitHubApiService.GetUserRepositoryBranchesAsync(userTestToken, repositoryId))
                .Select(branch => branch.Name);
            return Json(branchesNames);
        }
    }
}