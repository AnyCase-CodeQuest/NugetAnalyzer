using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGitHubApiService gitHubApiService;
        private const string userTestToken = "94c69698f37458f99db360c4e8d2803b5d6ba146";

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

        public async Task<JsonResult> Branches(long repositoryId)
        {
            var branchesNames = (await gitHubApiService.GetUserRepositoryBranchesAsync(userTestToken, repositoryId))
                .Select(b => b.Name);
            return Json(branchesNames);
        }
    }
}