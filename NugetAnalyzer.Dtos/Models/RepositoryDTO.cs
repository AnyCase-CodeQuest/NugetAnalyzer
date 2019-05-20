using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models
{
    public class RepositoryDTO : BaseRepositoryModel
    {
        public RepositoryDTO()
        {
            Solutions = new List<SolutionDTO>();
        }

        public RepositoryDTO(string name, string path, ICollection<SolutionDTO> solutions)
            : base(name, path)
        {
            Solutions = solutions;
        }

        public ICollection<SolutionDTO> Solutions { get; set; }
    }
}