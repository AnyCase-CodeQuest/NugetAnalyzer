using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IRepositoriesRepository : IRepository<Repository>
    {
        Task<IReadOnlyCollection<Repository>> GetRepositoriesWithIncludesAsync(Expression<Func<Repository, bool>> expression);

        Task<IReadOnlyCollection<string>> GetRepositoriesNamesAsync(Expression<Func<Repository, bool>> expression);
    }
}
