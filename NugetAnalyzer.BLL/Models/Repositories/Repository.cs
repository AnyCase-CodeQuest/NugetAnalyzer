using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Solutions;

namespace NugetAnalyzer.BLL.Models.Repositories
{
    public class Repository : BaseModel
    {
        public Repository()
            : base()
        {
            Solutions = new List<Solution>();
        }

        public Repository(string name, string path, IList<Solution> solutions)
            : base(name, path)
        {
            this.Solutions = solutions;
        }

        public IList<Solution> Solutions { get; set; }
    }
}