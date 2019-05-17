using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models.Reports;

namespace NugetAnalyzer.BLL.Interfaces
{
    /// <summary>
    /// Service to get information about repositories
    /// </summary>
    public interface IRepositoryService
    {
        /// <summary>
        /// Get information about repositories with embedded information about solutions, projects, and packages
        /// analyzed by package version, date, and obsolete.
        /// </summary>
        /// <param name="expression">Linq lambda expressions filter</param>
        /// <returns>A collection of analyzed repositories</returns>
        Task<ICollection<RepositoryWithVersionReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression);
    }
}
