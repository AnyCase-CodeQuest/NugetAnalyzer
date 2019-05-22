using System.Linq;
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
            return await DbSet
                .AsNoTracking()
                .Include(project => project.ProjectPackageVersions)
                .ThenInclude(projectPackageVersion => projectPackageVersion.PackageVersion)
                .ThenInclude(packageVersion => packageVersion.Package)
                .Where(project => project.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}