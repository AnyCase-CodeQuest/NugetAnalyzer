using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositoryService repositoryService;
        private readonly IUnitOfWork uow;
        public HomeController(IRepositoryService repositoryService, IUnitOfWork uow)
        {
            this.repositoryService = repositoryService;
            this.uow = uow;
        }
        public async Task<IActionResult> Index()
        {
            var result = await repositoryService.GetAnalyzedRepositoriesAsync(r => r.UserId == 1);
            return View();
        }
    }
}