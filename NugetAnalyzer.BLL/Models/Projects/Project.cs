using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Packages;

namespace NugetAnalyzer.BLL.Models.Projects
{
    public class Project : BaseModel
    {
        public Project()
            : base()
        {
            Packages = new List<Package>();
        }

        public Project(string name, string path, IList<Package> packages)
            : base(name, path)
        {
            this.Packages = packages;
        }

        public IList<Package> Packages { get; set; }
    }
}