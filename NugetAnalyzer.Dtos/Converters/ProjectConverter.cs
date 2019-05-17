using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

namespace NugetAnalyzer.Dtos.Converters
{
	public static class ProjectConverter
	{
		public static ProjectWithVersionReport ProjectToProjectVersionReport(Project project)
		{
			return project == null
				? null
				: new ProjectWithVersionReport
				{
					Id = project.Id,
					Name = project.Name
				};
		}
	}
}
