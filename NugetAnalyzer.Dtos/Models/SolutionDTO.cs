using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models
{
    public class SolutionDTO : BaseRepositoryModel
    {
        public SolutionDTO()
        {
            Projects = new List<ProjectDTO>();
        }

        public SolutionDTO(string name, string path, ICollection<ProjectDTO> projects)
            : base(name, path)
        {
            Projects = projects;
        }

        public ICollection<ProjectDTO> Projects { get; set; }
    }
}