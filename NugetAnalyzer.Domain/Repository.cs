using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class Repository
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<Solution> Solutions { get; set; }
    }
}
