using System.Collections.Generic;

namespace NugetAnalyzer.Dtos
{
    public class ProjectViewModel
    {
        public string ProjectName { get; set; }
        public IList<PackageViewModel> Packages { get; set; }
    }
}
