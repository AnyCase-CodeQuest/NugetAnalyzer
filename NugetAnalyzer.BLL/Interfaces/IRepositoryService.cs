using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models.Repositories;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IRepositoryService
    {
        Task<ICollection<RepositoryWithVersionReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression);
    }
}
