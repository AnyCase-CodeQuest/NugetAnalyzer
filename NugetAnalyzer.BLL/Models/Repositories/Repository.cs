using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Solutions;

namespace NugetAnalyzer.BLL.Models.Repositories
{
    public class Repository
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Solution> Solutions { get; set; }
    }
}