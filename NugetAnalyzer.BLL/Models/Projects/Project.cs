using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Packages;

namespace NugetAnalyzer.BLL.Models.Projects
{
    public class Project
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Package> Packages { get; set; }
    }
}