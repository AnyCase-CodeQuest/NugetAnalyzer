using NugetAnalyzer.Dtos.Models.Repositories;
using Octokit;

namespace NugetAnalyzer.Dtos.Converters
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
                    Url = repository.HtmlUrl,
                    Name = repository.Name
                };
        }
    }
}
