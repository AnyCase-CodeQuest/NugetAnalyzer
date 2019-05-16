using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositoryService repositoryService;
        public HomeController(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await repositoryService.GetAnalyzedRepositoriesAsync(repository => repository.UserId == 1);
            return View();
        }
    }
}