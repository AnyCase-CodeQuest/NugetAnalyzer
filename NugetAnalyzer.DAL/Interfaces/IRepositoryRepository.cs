using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IRepositoryRepository
    {
        Task<IReadOnlyCollection<Repository>> GetRepositoriesWithIncludesAsync(Expression<Func<Repository, bool>> expression);
    }
}
