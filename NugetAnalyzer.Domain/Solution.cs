using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Solution
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RepositoryId { get; set; }

        public Repository Repository { get; set; }

        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
