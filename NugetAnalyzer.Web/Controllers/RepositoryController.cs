using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models.Repositories;

namespace NugetAnalyzer.Web.Controllers
{
    public class RepositoryController : Controller
    {
        private readonly IGitHubApiService gitHubApiService;
        private readonly IRepositoryService repositoryService;
        private const string userTestToken = "d190dcfa7984739296f48fc9e87e021ec3ef76ec";

        public RepositoryController(IGitHubApiService gitHubApiService, IRepositoryService repositoryService)
        {
            this.gitHubApiService = gitHubApiService ?? throw new ArgumentNullException(nameof(gitHubApiService));
            this.repositoryService = repositoryService ?? throw new ArgumentNullException(nameof(repositoryService));
        }

        [HttpGet]
        public async Task<PartialViewResult> AddRepositories()
        {
            Task<IReadOnlyCollection<RepositoryChoice>> userGitHubRepositoriesTask = gitHubApiService.GetUserRepositoriesAsync(userTestToken);
            Task<IReadOnlyCollection<string>> addedUserRepositoriesNamesTask = repositoryService.GetRepositoriesNamesAsync(repository => repository.UserId == 1); // TODO: userId ?

            await Task.WhenAll(userGitHubRepositoriesTask, addedUserRepositoriesNamesTask);
            var userGitHubRepositories = await userGitHubRepositoriesTask;
            var addedUserRepositoriesNames = await addedUserRepositoriesNamesTask;

            IEnumerable<RepositoryChoice> notAddedUserRepositories = userGitHubRepositories
                .Where(repositoryChoice => !addedUserRepositoriesNames
                    .Contains(repositoryChoice.Name));

            return PartialView("RepositoriesPopUp", notAddedUserRepositories);
        }

        [HttpPost]
        public async Task<PartialViewResult> AddRepositories(Dictionary<string, string> repositories)
        {
            await repositoryService.AddRepositoriesAsync(repositories, userTestToken);
            var userRepositories = await repositoryService.GetAnalyzedRepositoriesAsync(repository => repository.UserId == 1);  // TODO: userId ?
            return PartialView("PartialRepositories", userRepositories);
        }

        [HttpGet]
        public async Task<JsonResult> Branches(long repositoryId)
        {
            IEnumerable<string> branchesNames = (await gitHubApiService.GetUserRepositoryBranchesAsync(userTestToken, repositoryId))
                .Select(branch => branch.Name);
            return Json(branchesNames);
        }
    }
}