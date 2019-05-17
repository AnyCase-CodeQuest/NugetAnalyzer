using System;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

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
	}
}
