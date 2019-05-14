using NugetAnalyzer.BLL.Models.Repositories;
using Octokit;

namespace NugetAnalyzer.BLL.Converters
{
    public static class RepositoryConverter
    {
        public static RepositoryChoice OctokitRepositoryToRepositoryChoice(Repository repository)
        {
            return repository == null
                ? null
                : new RepositoryChoice
                {
                    Id = repository.Id,
                    Name = repository.Name
                };
        }
    }
}
