using System;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

namespace NugetAnalyzer.Dtos.Converters
{
	public static class RepositoryConverter
	{
		public static RepositoryWithVersionReport RepositoryToRepositoryVersionReport(Repository repository)
		{
			return repository == null
				? null
				: new RepositoryWithVersionReport
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
	}
}
