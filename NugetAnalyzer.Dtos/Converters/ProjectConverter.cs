using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.DTOs.Converters
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
