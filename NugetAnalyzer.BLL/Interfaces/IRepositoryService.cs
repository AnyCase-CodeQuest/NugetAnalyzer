using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Models.Reports;
using NugetAnalyzer.DTOs.Models.Repositories;

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
        Task<ICollection<RepositoryReport>> GetAnalyzedRepositoriesAsync(Expression<Func<Repository, bool>> expression);

        /// <summary>
        /// Get repositories names  
        /// </summary>
        /// <param name="expression">Linq lambda expressions filter</param>
        /// <returns>A collection of repositories names</returns>
        Task<IReadOnlyCollection<string>> GetRepositoriesNamesAsync(Expression<Func<Repository, bool>> expression);

        /// <summary>
        /// Added git repositories for storing by user access token
        /// </summary>
        /// <param name="repositories">A dictionary which contains the repository URL as a key and the repository branch as a value</param>
        /// <param name="userToken">User git access token</param>
        /// <param name="userId">User identifier in DB</param>
        /// <returns>Response with a response type and collection of names of added repositories</returns>
        Task<AddRepositoriesResponseDTO> AddRepositoriesAsync(Dictionary<string, string> repositories, string userToken, int userId);
    }
}
