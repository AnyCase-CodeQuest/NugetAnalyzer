using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Solution : BaseEntity
    {
        public string Name { get; set; }

        public int RepositoryId { get; set; }

        public Repository Repository { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
