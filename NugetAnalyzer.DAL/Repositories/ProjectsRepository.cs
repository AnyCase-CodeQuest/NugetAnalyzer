using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NugetAnalyzer.DAL.Context;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.DAL.Repositories
{
    public class ProjectsRepository : Repository<Project>, IProjectsRepository
    {
        public ProjectsRepository(NugetAnalyzerDbContext context) : base(context)
        {
        }

        public async Task<Project> GetByIdWithIncludedPackageAsync(int id)
        {
           return await GetIQueryableIncludePackage(project => project.Id == id).SingleOrDefaultAsync();
        }

        private  IQueryable<Project> GetIQueryableIncludePackage(Expression<Func<Project, bool>> predicate)
        {
            return DbSet
                .AsNoTracking()
                .Include(project => project.ProjectPackageVersions)
                .ThenInclude(projectPackageVersion => projectPackageVersion.PackageVersion)
                .ThenInclude(packageVersion => packageVersion.Package)
                .Where(predicate);
        }
    }
}