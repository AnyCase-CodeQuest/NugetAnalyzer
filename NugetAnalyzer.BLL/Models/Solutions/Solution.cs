using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Projects;

namespace NugetAnalyzer.BLL.Models.Solutions
{
    public class Solution
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Project> Projects { get; set; }
    }
}