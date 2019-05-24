using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.DTOs.Models.Reports;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;

namespace NugetAnalyzer.Web.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IRepositoryService repositoryService;
        private readonly HttpContextInfoProvider httpContextInfoProvider;
        private readonly ISourceService sourceService;
        private readonly IProfileService profileService;

        public RepositoryController(
            IRepositoryService repositoryService,
            HttpContextInfoProvider httpContextInfoProvider,
            ISourceService sourceService,
            IProfileService profileService)
        {
            this.repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));
            this.httpContextInfoProvider = httpContextInfoProvider ?? throw new ArgumentNullException(nameof(httpContextInfoProvider));
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        }

        [HttpGet]
        public async Task<IActionResult> Report()
        {
            string sourceName = httpContextInfoProvider.GetSourceName();
            int externalId = httpContextInfoProvider.GetExternalId();
            int sourceId = await sourceService.GetSourceIdByName(sourceName);

            ProfileDTO profile =  await profileService.GetProfileBySourceIdAsync(sourceId, externalId);

            ICollection<RepositoryReport> model = await repositoryService.GetAnalyzedRepositoriesAsync(p => p.UserId == profile.UserId);
            
            return View("_RepositoriesReport", model);
        }
    }
}