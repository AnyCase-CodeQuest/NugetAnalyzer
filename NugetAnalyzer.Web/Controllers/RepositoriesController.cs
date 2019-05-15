using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models.Repositories;

namespace NugetAnalyzer.Web.Controllers
{
    public class RepositoriesController : Controller
    {
        private readonly IRepositoryService repositoryService;

        public RepositoriesController(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }
        public IEnumerable<RepositoryWithVersionReport> Overview()
        {
            return null;
        }

        public async Task<PartialViewResult> Repositories(int userId)
        {
            var result = await repositoryService.GetAnalyzedRepositoriesAsync(p => p.UserId == userId);
            
            return PartialView(result);
        }
    }
}