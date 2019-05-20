using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Interfaces
{
    public interface IProjectsRepository : IRepository<Project>
    {
        Task<IReadOnlyCollection<Project>> GetCollectionIncludePackage(Expression<Func<Project, bool>> predicate);
    }
}