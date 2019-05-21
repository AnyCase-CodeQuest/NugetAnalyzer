using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;

namespace NugetAnalyzer.DTOs.Converters
{
	public static class ProjectConverter
	{
		public static ProjectReport ProjectToProjectVersionReport(Project project)
		{
			return project == null
				? null
				: new ProjectReport
				{
					Id = project.Id,
					Name = project.Name
				};
		}
	}
}
