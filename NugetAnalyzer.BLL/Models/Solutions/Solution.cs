using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Projects;

namespace NugetAnalyzer.BLL.Models.Solutions
{
    public class Solution : BaseModel
    {
        public Solution()
            : base()
        {
            Projects = new List<Project>();
        }

        public Solution(string name, string path, IList<Project> projects)
            : base(name, path)
        {
            this.Projects = projects;
        }

        public IList<Project> Projects { get; set; }
    }
}