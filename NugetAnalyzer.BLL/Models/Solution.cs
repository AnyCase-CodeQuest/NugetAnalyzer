using System.Collections.Generic;

namespace NugetAnalyzer.BLL.Models
{
    public class Solution
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IList<Project> Projects { get; set; }
    }
}