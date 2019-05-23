using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DTOs.Models;
using NugetAnalyzer.DTOs.Models.Reports;
using NugetAnalyzer.Dtos.Models.Repositories;
using NugetAnalyzer.DTOs.Models.Repositories;
using NugetAnalyzer.Web.Infrastructure.HttpAccessors;

namespace NugetAnalyzer.Web.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IGitHubApiService gitHubApiService;
        private readonly IRepositoryService repositoryService;
        private readonly IProfileService profileService;
        private readonly ISourceService sourceService;
        private readonly HttpContextInfoProvider httpContextInfoProvider;

        public RepositoryController(IGitHubApiService gitHubApiService,
            IRepositoryService repositoryService,
            IProfileService profileService, 
            ISourceService sourceService,
            HttpContextInfoProvider httpContextInfoProvider)
        {
            this.gitHubApiService = gitHubApiService ?? throw new ArgumentNullException(nameof(gitHubApiService));
            this.repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));
            this.profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
            this.sourceService = sourceService ?? throw new ArgumentNullException(nameof(sourceService));
            this.httpContextInfoProvider = httpContextInfoProvider ?? throw new ArgumentNullException(nameof(httpContextInfoProvider));
        }

        [HttpGet]
        public async Task<PartialViewResult> AddRepositories()
        {
            ProfileDTO userProfile = await GetCurrentUserProfileAsync();

            Task<IReadOnlyCollection<RepositoryChoice>> userGitHubRepositoriesTask = gitHubApiService
                .GetUserRepositoriesAsync(userProfile.AccessToken);
            Task<IReadOnlyCollection<string>> addedUserRepositoriesNamesTask = repositoryService
                .GetRepositoriesNamesAsync(repository => repository.UserId == userProfile.UserId);

            await Task.WhenAll(userGitHubRepositoriesTask, addedUserRepositoriesNamesTask);
            var userGitHubRepositories = await userGitHubRepositoriesTask;
            var addedUserRepositoriesNames = await addedUserRepositoriesNamesTask;

            IEnumerable<RepositoryChoice> notAddedUserRepositories = userGitHubRepositories
                .Where(repositoryChoice => !addedUserRepositoriesNames
                    .Contains(repositoryChoice.Name));

            return PartialView("AddRepositoriesPopUp", notAddedUserRepositories);
        }

        [HttpPost]
        public async Task<PartialViewResult> AddRepositories([FromBody]AddRepositoriesRequest addRepositoriesRequestModel)
        {
            ProfileDTO userProfile = await GetCurrentUserProfileAsync();

            var addedRepositoriesResponse = await repositoryService
                .AddRepositoriesAsync(addRepositoriesRequestModel.Repositories, userProfile.AccessToken, userProfile.UserId);
            ICollection<RepositoryReport> userRepositories = await repositoryService
                .GetAnalyzedRepositoriesAsync(repository => repository.UserId == userProfile.UserId);

            return addRepositoriesRequestModel.IsFromLayout
                ? PartialView("AddRepositoriesResponsePopUp", addedRepositoriesResponse)
                : PartialView("_RepositoriesReport", userRepositories);
        }

        [HttpGet]
        public async Task<JsonResult> Branches(long repositoryId)
        {
            ProfileDTO userProfile = await GetCurrentUserProfileAsync();

            IEnumerable<string> branchesNames = (await gitHubApiService
                    .GetUserRepositoryBranchesAsync(userProfile.AccessToken, repositoryId))
                .Select(branch => branch.Name);
            return Json(branchesNames);
        }

        private async Task<ProfileDTO> GetCurrentUserProfileAsync()
        {
            string sourceName = httpContextInfoProvider.GetSourceName();
            int externalId = httpContextInfoProvider.GetExternalId();
            int sourceId = await sourceService.GetSourceIdByName(sourceName);

            return await profileService.GetProfileBySourceIdAsync(sourceId, externalId);
        }
    }
}