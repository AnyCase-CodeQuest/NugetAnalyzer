using System;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepositoryService
    {
        private IUnitOfWork unitOfWork;
        private IRepositoryMapper repositoryMapper;

        public RepositoryService(IUnitOfWork unitOfWork, IRepositoryMapper repositoryMapper)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.repositoryMapper = repositoryMapper ?? throw new ArgumentNullException(nameof(repositoryMapper));
        }

        public async void SaveAsync(Repository repository, int userId)
        {;
            unitOfWork.GetRepository<Domain.Repository>().Add(await repositoryMapper.ToDomainAsync(repository, userId));
            unitOfWork.SaveChangesAsync();
        }
    }
}
