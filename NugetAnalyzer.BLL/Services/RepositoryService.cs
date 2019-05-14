using System;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models.Repositories;
using NugetAnalyzer.DAL.Interfaces;

namespace NugetAnalyzer.BLL.Services
{
    public class RepositoryService : IRepositoryService
    {
        private IUnitOfWork unitOfWork;

        public RepositoryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async void SaveAsync(Repository repository, int userId)
        {
            var mapper = new RepositoryMapper(unitOfWork);
            unitOfWork.GetRepository<Domain.Repository>().Add(await mapper.ToDomainAsync(repository, userId));
            unitOfWork.SaveChangesAsync();
        }
    }
}
