using System;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos;

namespace NugetAnalyzer.BLL.Services
{
    class ProjectService : IProjectService
    {
        private readonly IRepository<Project> projectRepository;
        private readonly IUnitOfWork unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(projectRepository));
            projectRepository = unitOfWork.GetRepository<Project>();
        }
        public async Task<ProjectViewModel> GetProjectById(int projectId)
        {
            return null;
        }
    }
}
