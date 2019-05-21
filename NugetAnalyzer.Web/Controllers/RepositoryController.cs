using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.Web.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IRepositoryService repositoryService;

        public RepositoryController(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }

        public async Task<IActionResult> Index()
        {
           ICollection<RepositoryReport> repositories = await repositoryService.GetAnalyzedRepositoriesAsync(p => p.UserId == 3);

            return View(repositories);
        }
    }
}