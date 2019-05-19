using System.Collections.Generic;
using NugetAnalyzer.BLL.Models.Solutions;

namespace NugetAnalyzer.BLL.Models.Repositories
{
    public class Repository : BaseRepositoryModel
    {
        public Repository()
            : base()
        {
            Solutions = new List<Solution>();
        }

        public Repository(string name, string path, ICollection<Solution> solutions)
            : base(name, path)
        {
            this.Solutions = solutions;
        }

        public ICollection<Solution> Solutions { get; set; }
    }
}