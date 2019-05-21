using System;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.DTOs.Converters
{
	public static class RepositoryConverter
	{
		public static RepositoryReport RepositoryToRepositoryVersionReport(Repository repository)
		{
			return repository == null
				? null
				: new RepositoryReport
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
