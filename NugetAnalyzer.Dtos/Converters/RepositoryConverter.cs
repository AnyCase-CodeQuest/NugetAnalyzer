using System;
using System.Linq;
using NugetAnalyzer.Dtos.Models.Reports;
using NugetAnalyzer.Dtos.Models.Repositories;

namespace NugetAnalyzer.Dtos.Converters
{
	public static class RepositoryConverter
	{
		public static RepositoryVersionReport RepositoryToRepositoryVersionReport(Repository repository)
		{
			return repository == null
				? null
				: new RepositoryVersionReport
					{
						Id = repository.Id,
						Name = repository.Name,
						Solutions = repository.Solutions == null
								? throw new ArgumentNullException(nameof(repository.Solutions))
								: repository.Solutions
									.Select(SolutionConverter.SolutionToSolutionVersionReport)
									.ToList()
					};
		}

        public static RepositoryChoice OctokitRepositoryToRepositoryChoice(Octokit.Repository repository)
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
