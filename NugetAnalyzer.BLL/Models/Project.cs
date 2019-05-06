using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Models
{
    public class Project
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Package> Packages { get; set; }
    }
}