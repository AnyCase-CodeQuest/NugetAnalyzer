using System;
using NugetAnalyzer.BLL.Interfaces;
using LibGit2Sharp;

namespace NugetAnalyzer.BLL.Services
{
    public class GitService : IGitService
    {
        public void CloneBranch(
            string repositoryUrl,
            string path,
            string userToken,
            string branchName = "master")
        {
            if (repositoryUrl == null)
            {
                throw new ArgumentNullException(nameof(repositoryUrl));
            }
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (userToken == null)
            {
                throw new ArgumentNullException(nameof(userToken));
            }
            if (branchName == null)
            {
                throw new ArgumentNullException(nameof(branchName));
            }

            var cloneOptions = new CloneOptions
            {
                CredentialsProvider = (url, user, cred) =>
                    new UsernamePasswordCredentials
                    {
                        Username = userToken,
                        Password = string.Empty
                    },
                BranchName = branchName
            };

            Repository.Clone(repositoryUrl, path, cloneOptions);
        }
    }
}
