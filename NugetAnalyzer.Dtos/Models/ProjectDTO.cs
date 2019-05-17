using System.Collections.Generic;

namespace NugetAnalyzer.DTOs.Models
{
    public class ProjectDTO : BaseRepositoryModel
    {
        public ProjectDTO()
        {
            Packages = new List<PackageDTO>();
        }

        public ProjectDTO(string name, string path, ICollection<PackageDTO> packages)
            : base(name, path)
        {
            Packages = packages;
        }

        public ICollection<PackageDTO> Packages { get; set; }
    }
}