using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Models
{
    public class Repository
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Solution> Solutions { get; set; }
    }
}