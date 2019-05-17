using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

namespace NugetAnalyzer.Dtos.Converters
{
	public static class ProjectConverter
	{
		public static ProjectVersionReport ProjectToProjectVersionReport(Project project)
		{
			return project == null
				? null
				: new ProjectVersionReport
				{
					Id = project.Id,
					Name = project.Name
				};
		}
	}
}
